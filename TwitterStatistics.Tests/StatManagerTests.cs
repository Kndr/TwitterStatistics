using Microsoft.VisualStudio.TestTools.UnitTesting;
using Processor.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using TwitterStatistics.Managers;

namespace TwitterStatistics.Tests
{
    [TestClass]
    public class StatisticsManagerTests
    {
        [TestMethod]
        public void StartTime_Test()
        {
            TweetStat tweetStat = new TweetStat();
            var StatisticsManager = new StatisticsManager(tweetStat);

            tweetStat.EndDateTime = DateTime.Now.AddHours(1);

            StatisticsManager.StartTime();

            Assert.IsNull(tweetStat.EndDateTime);
            Assert.AreNotEqual(default, tweetStat.StartDateTime);
        }

        [TestMethod]
        public void StopTime_When_EndDateTime_IsNull_Test()
        {
            TweetStat tweetStat = new TweetStat();
            var StatisticsManager = new StatisticsManager(tweetStat);

            StatisticsManager.StopTime();

            Assert.IsNotNull(tweetStat.EndDateTime);
        }

        [TestMethod]
        public void StopTime_When_EndDateTime_Not_Null_Test()
        {
            TweetStat tweetStat = new TweetStat();
            var StatisticsManager = new StatisticsManager(tweetStat);

            var endDate = DateTime.Now.AddMinutes(-61);
            tweetStat.EndDateTime = endDate;

            StatisticsManager.StopTime();
            Assert.AreEqual(endDate, tweetStat.EndDateTime);
        }

        [TestMethod]
        public async Task GetStatisticsAsync()
        {
            var tweetStat = Utility.GetTestData2();
            var StatisticsManager = new StatisticsManager(tweetStat);

            var statistics = await StatisticsManager.GetStatisticsAsync();
            Assert.AreEqual(tweetStat.TweetCount, statistics.TotalTweets);
            Assert.AreEqual(string.Format("{0:P2}", (double)tweetStat.TweetCountWithEmojis/statistics.TotalTweets), statistics.TweetsWithEmojisPercent);
            Assert.AreEqual(string.Format("{0:P2}", (double)tweetStat.TweetCountWithUrls / statistics.TotalTweets), statistics.TweetsWithUrlPercent);
            Assert.AreEqual(string.Format("{0:P2}", (double)tweetStat.TweetCountWithPhotoUrls / statistics.TotalTweets), statistics.TweetsWithPhotoUrlPercent);
            Assert.AreEqual($"{tweetStat.TweetCountWithHashtags}: {tweetStat.Hashtags.First().Key}", statistics.TopHashtags.First());
            Assert.IsTrue(statistics.TopEmojis.First().Contains(tweetStat.Emojis.First().Key));
            Assert.IsTrue(statistics.TopEmojis.First().Contains(tweetStat.Emojis.First().Value.ToString()));
            Assert.AreEqual($"{tweetStat.Domains.First().Value}: {tweetStat.Domains.First().Key}", statistics.TopDomains.First());
        }
    }
}
