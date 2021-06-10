using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Processor.Client;
using Processor.Entities;
using Processor.Managers;
using Processor.Services;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TwitterStatistics.Managers;

namespace TwitterStatistics.Tests
{
    [TestClass]
    public class PerformanceTests
    {
        [TestMethod]
        public async Task Measure_Performance_Test()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            //prepare tweet manager to accept a stream, read tweets and enqueue them into the queue
            using Stream stream = new MemoryStream();

            var httpClientWrapper = new Mock<HttpClientWrapper>("Brearer Token");
            httpClientWrapper
                .Setup(m => m.GetStreamAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(stream);

            var queue = new ConcurrentQueue<string>();
            var _options = Options.Create(new TwitterServiceOptions() { ServiceUrl = It.IsAny<string>(), BearerToken = It.IsAny<string>() });
            var tweetManager = new TweetManager(queue, _options, httpClientWrapper.Object);


            //prepare QueueBackgroundService to process tweets from queue
            TweetStat tweetStat = new TweetStat();
            var queueService = new QueueBackgroundService(queue, tweetStat);

            var cancellationTokenSource = new CancellationTokenSource();
            var stoppingToken = cancellationTokenSource.Token;

            //start work
            var sw = Stopwatch.StartNew();

            var arrayData = Utility.GetTestTweets().Select(tw => new { Data = new { Text = tw } });//13 sample tweets
            var multiplier = 100;
            for (int i = 0; i < multiplier; i++)
            {
                foreach (var tweet in arrayData)
                {
                    await JsonSerializer.SerializeAsync(stream, tweet, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            stream.Position = 0;

            tweetManager.StartAsync();//Note: don't add await
            await queueService.StartAsync(stoppingToken);

            await Task.Delay(500);
            tweetManager.Stop();//stop accepting tweets

            var StatisticsManager = new StatisticsManager(tweetStat);
            var statistics = await StatisticsManager.GetStatisticsAsync();

            await Task.Delay(500);

            cancellationTokenSource.Cancel();//cancell queue processing

            sw.Stop();

            Assert.AreEqual(arrayData.Count()* multiplier, tweetStat.TweetCount);
            Assert.AreEqual(arrayData.Count()* multiplier, statistics.TotalTweets);

            Debug.WriteLine($"Sent and processed {tweetStat.TweetCount} tweets in {sw.ElapsedMilliseconds/1000} seconds.");
        }
    }
}
