using Processor.Entities;
using System;

namespace TwitterStatistics.Tests
{
    public class Utility
    {
        /// <summary>
        /// Get TweetStat object with data
        /// </summary>
        /// <returns></returns>
        public static TweetStat GetTestData()
        {
            var tweetStat = new TweetStat();

            tweetStat.TweetCount = 10;
            tweetStat.TweetCountWithEmojis = 1;
            tweetStat.TweetCountWithUrls = 2;
            tweetStat.TweetCountWithPhotoUrls = 3;
            tweetStat.TweetCountWithHashtags = 4;
            tweetStat.Emojis.AddOrUpdate("test1", 1, (k, c) => 1);
            tweetStat.Hashtags.AddOrUpdate("test2", 2, (k, c) => 2);
            tweetStat.Domains.AddOrUpdate("test3", 3, (k, c) => 3);
            tweetStat.StartDateTime = DateTime.Now.AddHours(1);
            tweetStat.EndDateTime = null;

            return tweetStat;
        }

        /// <summary>
        /// Get TweetStat object with data (with emoji and url)
        /// </summary>
        /// <returns></returns>
        public static TweetStat GetTestData2()
        {
            var tweetStat = new TweetStat();

            tweetStat.TweetCount = 1000;
            tweetStat.TweetCountWithEmojis = 10;
            tweetStat.TweetCountWithUrls = 20;
            tweetStat.TweetCountWithPhotoUrls = 3;
            tweetStat.TweetCountWithHashtags = 4;

            tweetStat.Emojis.AddOrUpdate("😶", 10, (k, c) => 10);
            tweetStat.Hashtags.AddOrUpdate("#test", 4, (k, c) => 4);
            tweetStat.Domains.AddOrUpdate("http://test.com", 20, (k, c) => 20);

            tweetStat.StartDateTime = DateTime.Now.AddHours(1);
            tweetStat.EndDateTime = null;

            return tweetStat;
        }

        public static string GetTweetWithThreeEmojis()
        {
            return @"
                {
                    ""data"": {
                    ""id"": ""1400259150370779136"",
                    ""text"": ""Waiting for you opinion 😶😶😶 #opinion https://t.co/8F9ePrzw95""
                    }
                }";
        }

        public static string[] GetTestTweets()
        {
            return new string[] 
            {
                "@Fireturtle1000 https://t.co/8F9ePrzw95",
                "CHAU WJFMWLDDNWJBAJAJAJJAAAAJ https://t.co/z7JxLClqKv",
                "@madilenha BBB (tenho quase certeza 😁). Também fiquei nessa dúvida. Minha ficha só caiu qdo vi os dois juntos.",
                "@Zwshis @darlingshines Foi com bolsa ou sem bolsa?",
                "@David25609 omg 😭",
                "QDレーザ!! (○'ω'σ)Σ≡Σ≡Σ ≡Σ≡",
                "RT @gulfkanawut: อย่าลืมไปทานกันนะงับบบ😋 #BucherMarbleBaconxGulf",
                "RT @g_g_QQyy: لُطف الرد ، يبني الودّ . https://t.co/DWZs9wOOG7",
                "RT @jimintoday__: 맥도날드 TikTok\nhttps://t.co/km1vjwGyhO\n#BTS #방탄소년단 @BTS_twt https://t.co/guvBz1769i",
                "RT @dreamdates_: ✧*。— Update!\nDream added \"hi I'm Dream :)\" to his Instagram bio! https://t.co/JKikj4WrBb",
                "RT @TubboTWO: Little gamer boi!!! https://t.co/BHjdf1O3uK",
                "@jiminrkve @BTS_twt superior \n#ThePurgePark #TeamPromise #박지민 #지민 #JIMIN #방탄소년단지민 ジミン @BTS_twt https://t.co/u220xvo243",
                "RT @nantawan_rt: มารอชม TVC กันฮะ น้อนนกลัฟจะน่ารักขนาดไหนน้าา ตื่นเต้นจุงเบยยย🥰💖 @gulfkanawut \n\n#BucherMarbleBaconxGulf https://t.co/3cVMX…"
            };
        }
    }
}
