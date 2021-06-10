using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace TwitterStatistics.Tests
{
    [TestClass]
    public class TweetStatTests
    {
        [TestMethod]
        public void Clone_Test()
        {
            var tweetStat = Utility.GetTestData();

            var clone = tweetStat.Clone();

            tweetStat.TweetCount = 100;
            tweetStat.TweetCountWithEmojis = 10;
            tweetStat.TweetCountWithUrls = 20;
            tweetStat.TweetCountWithPhotoUrls = 30;
            tweetStat.TweetCountWithHashtags = 40;
            tweetStat.Emojis.AddOrUpdate("test10", 10, (k,c)=>10);
            tweetStat.Hashtags.AddOrUpdate("test20", 20, (k, c) => 20);
            tweetStat.Domains.AddOrUpdate("test30", 30, (k, c) => 30);
            tweetStat.StartDateTime = DateTime.Now.AddHours(10);
            tweetStat.EndDateTime = DateTime.Now.AddHours(11); ;

            Assert.AreNotEqual(clone.TweetCount, tweetStat.TweetCount);
            Assert.AreNotEqual(clone.TweetCountWithEmojis, tweetStat.TweetCountWithEmojis);
            Assert.AreNotEqual(clone.TweetCountWithUrls, tweetStat.TweetCountWithUrls);
            Assert.AreNotEqual(clone.TweetCountWithPhotoUrls, tweetStat.TweetCountWithPhotoUrls);
            Assert.AreNotEqual(clone.TweetCountWithHashtags, tweetStat.TweetCountWithHashtags);
            Assert.AreNotEqual(clone.Emojis.Count(), tweetStat.Emojis.Count());
            Assert.AreNotEqual(clone.Hashtags.Count(), tweetStat.Hashtags.Count());
            Assert.AreNotEqual(clone.Domains.Count(), tweetStat.Domains.Count());

            Assert.AreNotEqual(clone.StartDateTime, tweetStat.StartDateTime);
            Assert.AreNotEqual(clone.EndDateTime, tweetStat.EndDateTime);
        }

        [TestMethod]
        public void Reset_Test()
        {
            var tweetStat = Utility.GetTestData();
            tweetStat.Reset();

            Assert.AreEqual(0, tweetStat.TweetCount);
            Assert.AreEqual(0, tweetStat.TweetCountWithEmojis);
            Assert.AreEqual(0, tweetStat.TweetCountWithUrls);
            Assert.AreEqual(0, tweetStat.TweetCountWithPhotoUrls);
            Assert.AreEqual(0, tweetStat.TweetCountWithHashtags);
            Assert.AreEqual(0, tweetStat.Emojis.Count());
            Assert.AreEqual(0, tweetStat.Hashtags.Count());
            Assert.AreEqual(0, tweetStat.Domains.Count());

            Assert.AreEqual(default, tweetStat.StartDateTime);
            Assert.AreEqual(null, tweetStat.EndDateTime);
        }
    }
}
