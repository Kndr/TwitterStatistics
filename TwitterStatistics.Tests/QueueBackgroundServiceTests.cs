using Microsoft.VisualStudio.TestTools.UnitTesting;
using Processor.Entities;
using Processor.Services;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TwitterStatistics.Tests
{
    [TestClass]
    public class QueueBackgroundServiceTests
    {
        [TestMethod]
        public async Task QueueBackgroundService_Queue_Processing_Test()
        {
            ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
            TweetStat tweetStat = new TweetStat();

            var tweet = Utility.GetTweetWithThreeEmojis();
            queue.Enqueue(tweet);

            var queueService = new QueueBackgroundService(queue, tweetStat);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken stoppingToken = cancellationTokenSource.Token;
            await queueService.StartAsync(stoppingToken);

            await Task.Delay(500);

            cancellationTokenSource.Cancel();

            Assert.AreEqual(1, tweetStat.TweetCount);
            Assert.AreEqual(1, tweetStat.Hashtags.Count);
            Assert.AreEqual(1, tweetStat.Emojis.Count);
            Assert.AreEqual(3, tweetStat.Emojis.First().Value);
            Assert.AreEqual(1, tweetStat.TweetCountWithUrls);
            Assert.AreEqual(0, tweetStat.TweetCountWithPhotoUrls);
            Assert.AreEqual(1, tweetStat.Domains.Count);
        }
    }
}
