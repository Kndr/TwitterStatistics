using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Processor.Client;
using Processor.Entities;
using Processor.Managers;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TwitterStatistics.Tests
{
    [TestClass]
    public class TweetManagerTests
    {
        [TestMethod]
        public async Task Receive_Tweets_Test()
        {
            using Stream stream = new MemoryStream();

            var arrayData = Utility.GetTestTweets().Select(tw=> new { Data = new { Text = tw } });
            foreach (var tweet in arrayData)
            {
                await JsonSerializer.SerializeAsync(stream, tweet, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
            }
            stream.Position = 0;

            var httpClientWrapper = new Mock<HttpClientWrapper>("Brearer Token");
            httpClientWrapper
                .Setup(m => m.GetStreamAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(stream);

            var queue = new ConcurrentQueue<string>();
            var _options = Options.Create(new TwitterServiceOptions() { ServiceUrl = It.IsAny<string>(), BearerToken = It.IsAny<string>() });
            var tweetManager = new TweetManager(queue, _options, httpClientWrapper.Object);

            await tweetManager.StartAsync();

            tweetManager.Stop();

            Assert.AreEqual(arrayData.Count(), queue.Count);
        }
    }
}
