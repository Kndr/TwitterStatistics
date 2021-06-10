using Newtonsoft.Json;
using Processor.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Processor.Managers.Interfaces;

namespace TwitterStatistics.Managers
{
    public class StatisticsManager : IStatisticsManager
    {
        private readonly TweetStat _tweetStat;

        private static List<EmojiData> _emojiDataList;
        
        /// <summary>
        /// Collection of emoji data
        /// </summary>
        public static List<EmojiData> EmojiDataList
        {
            get
            {
                _emojiDataList ??= new List<EmojiData>();
                return _emojiDataList.Any() ? _emojiDataList : GetEmojiData().ToList();
            }
        }

        public StatisticsManager(TweetStat tweetStat) 
        {
            _tweetStat = tweetStat;
        }

        /// <summary>
        /// Set start time and reset statistics _tweetStat object
        /// </summary>
        public void StartTime()
        {
            if (_tweetStat.EndDateTime != null)//reset _tweetStat object if come here after StopTime was executed
            {
                _tweetStat.Reset();
            }
            _tweetStat.StartDateTime = DateTime.Now;
        }

        /// <summary>
        /// Set stop time
        /// </summary>
        public void StopTime()
        {
            if (_tweetStat.EndDateTime == null)//if it is not yet stopped
            {
                _tweetStat.EndDateTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Prepare statistics for UI
        /// </summary>
        /// <returns></returns>
        public async Task<TweetStatUi> GetStatisticsAsync()
        {
            TweetStat tweetStatCopy = _tweetStat.Clone();

            var stat = await Task.Run(() => CollectStatForUi(tweetStatCopy));
            return stat;
        }

        private static TweetStatUi CollectStatForUi(TweetStat tweetStat)
        {
            var span = (tweetStat.EndDateTime != null ? tweetStat.EndDateTime.Value : DateTime.Now) - tweetStat.StartDateTime;

            var stat = new TweetStatUi();

            stat.TotalTweets = tweetStat.TweetCount;
            stat.TopDomains = tweetStat.Domains.OrderByDescending(kv => kv.Value).Take(5).Select(kv => $"{kv.Value}: {kv.Key}").ToList();
            stat.TopEmojis = tweetStat.Emojis.OrderByDescending(kv => kv.Value).Take(5)
                .Select(kv =>
                {
                    return $"{kv.Value}: {ComposeEmojiWithName(kv.Key)}";
                }).ToList();

            stat.TopHashtags = tweetStat.Hashtags.OrderByDescending(kv => kv.Value).Take(5).Select(kv => $"{kv.Value}: {kv.Key}").ToList();
            stat.TweetsWithEmojisPercent = string.Format("{0:P2}", (double)tweetStat.TweetCountWithEmojis / stat.TotalTweets);
            stat.TweetsWithPhotoUrlPercent = string.Format("{0:P2}", (double)tweetStat.TweetCountWithPhotoUrls / stat.TotalTweets);
            stat.TweetsWithUrlPercent = string.Format("{0:P2}", (double)tweetStat.TweetCountWithUrls / stat.TotalTweets);
            stat.AvgPerSecond = string.Format("{0:N1}", tweetStat.TweetCount / span.TotalSeconds);
            stat.AvgPerMinute = string.Format("{0:N1}", tweetStat.TweetCount / span.TotalMinutes);
            stat.AvgPerHour = string.Format("{0:N1}", tweetStat.TweetCount / span.TotalHours);

            return stat;
        }

        private static string ComposeEmojiWithName(string emoji)
        {
            var chars = emoji.ToCharArray();
            var num = char.ConvertToUtf32(chars[0], chars[1]);
            var hex = string.Format("{0:X2}", num);

            var name = EmojiDataList.FirstOrDefault(e => e.Unified == hex)?.Name;
            return name != null ? $"{emoji} - {name}" : emoji; ;
        }

        private static IEnumerable<EmojiData> GetEmojiData()
        {
            if (!_emojiDataList.Any())
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                var reader = new JsonTextReader(new StreamReader(".//Data//emoji.json"));
                _emojiDataList = serializer.Deserialize<List<EmojiData>>(reader);

                Log.Information("Emoji data loaded");
            }

            return _emojiDataList;
        }
    }
}
