using System.Collections.Generic;

namespace Processor.Entities
{
    public class TweetStatUi
    {
        public int TotalTweets { get; set; }
        public string AvgPerHour { get; set; }
        public string AvgPerMinute { get; set; }
        public string AvgPerSecond { get; set; }
        public List<string> TopEmojis { get; set; }
        public List<string> TopHashtags { get; set; }
        public List<string> TopDomains { get; set; }
        public string TweetsWithEmojisPercent { get; set; }
        public string TweetsWithUrlPercent { get; set; }
        public string TweetsWithPhotoUrlPercent { get; set; }
    }
}
