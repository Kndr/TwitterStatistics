using Serilog;
using System;
using System.Collections.Concurrent;

namespace Processor.Entities
{
    public class TweetStat
    {
        public int TweetCount { get; set; }
        public int TweetCountWithEmojis { get; set; }
        public int TweetCountWithUrls { get; set; }
        public int TweetCountWithPhotoUrls { get; set; }
        public int TweetCountWithHashtags { get; set; }

        public ConcurrentDictionary<string, int> Emojis { get; set; } = new ConcurrentDictionary<string, int>();
        public ConcurrentDictionary<string, int> Hashtags { get; set; } = new ConcurrentDictionary<string, int>();
        public ConcurrentDictionary<string, int> Domains { get; set; } = new ConcurrentDictionary<string, int>();
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public TweetStat Clone()
        {
            var tweetStat = (TweetStat)MemberwiseClone();
            tweetStat.Emojis = new ConcurrentDictionary<string, int>(Emojis);
            tweetStat.Hashtags = new ConcurrentDictionary<string, int>(Hashtags);
            tweetStat.Domains = new ConcurrentDictionary<string, int>(Domains);

            Log.Information("TweetStat object cloned");
            return tweetStat;
        }

        public void Reset()
        {
            TweetCount = 0;
            TweetCountWithEmojis = 0;
            TweetCountWithUrls = 0;
            TweetCountWithPhotoUrls = 0;
            TweetCountWithHashtags = 0;
            Emojis = new ConcurrentDictionary<string, int>();
            Hashtags = new ConcurrentDictionary<string, int>();
            Domains = new ConcurrentDictionary<string, int>();
            StartDateTime = default;
            EndDateTime = null;

            Log.Information("TweetStat reset");
        }
    }
}
