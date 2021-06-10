using Microsoft.Extensions.Hosting;
using Processor.Entities;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Processor.Services
{
    public class QueueBackgroundService: BackgroundService
    {
        private static readonly Regex _hashtagRegex = new Regex(@"#\w+", RegexOptions.Compiled);
        private static readonly Regex _photoRegex = new Regex(@"instagram.com[/]|pic.twitter.com[/]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _urlRegex = new Regex(@"http[s]?[:/]\S+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _emojiRegex = new Regex("\ud83c[\udf00-\udfff]|\ud83d[\udc00-\ude4f]|\ud83d[\ude80-\udeff]", RegexOptions.Compiled);
        private static readonly Regex _domainRegex = new Regex(@"http[s]?[:/]+[^/]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly ConcurrentQueue<string> _queue;
        private readonly TweetStat _tweetStat;

        public QueueBackgroundService(ConcurrentQueue<string> queue, TweetStat tweetStat)
        {
            _queue = queue;
            _tweetStat = tweetStat;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Starting QueueBackgroundService");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queue.IsEmpty)
                {
                    await Task.Delay(15);
                    continue;
                }
                if (_queue.TryDequeue(out string twit))
                {
                    ProcessTwit(twit, _tweetStat);
                }
            }
        }

        private static void ProcessTwit(string twit, TweetStat tweetStat)
        {
            try
            {
                tweetStat.TweetCount++;

                //var sw = Stopwatch.StartNew();

                GetEmojis(twit, tweetStat);

                GetUrls(twit, tweetStat);

                GetPhotos(twit, tweetStat);

                GetHashtags(twit, tweetStat);

                //sw.Stop();
                //Debug.WriteLine($"Processing took: {sw.ElapsedTicks} ticks");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to process the tweet: {twit}");
            }
        }

        #region Processing elements of the twit

        private static void GetHashtags(string twit, TweetStat tweetStat)
        {
            var hashtags = _hashtagRegex.Matches(twit);

            if (hashtags.Count == 0)
                return;

            tweetStat.TweetCountWithHashtags++;

            foreach (var hashtag in hashtags)
            {
                tweetStat.Hashtags.AddOrUpdate(hashtag.ToString(), 1, (k, c) => { return c + 1; });
            }
        }

        private static void GetPhotos(string twit, TweetStat tweetStat)
        {
            var isMatch = _photoRegex.IsMatch(twit);

            if (isMatch)
            {
                tweetStat.TweetCountWithPhotoUrls++;
            }
        }

        private static void GetUrls(string twit, TweetStat tweetStat)
        {
            var urls = _urlRegex.Matches(twit);

            if (urls.Count == 0)
                return;

            tweetStat.TweetCountWithUrls++;

            var domains = _domainRegex.Matches(urls.Select(m=>m.Value).Aggregate((a,b)=>a = $"{a} {b}"));

            foreach (Match domain in domains)
            {
                tweetStat.Domains.AddOrUpdate(domain.Value, 1, (k, c) => { return c + 1; });
            }
        }

        private static void GetEmojis(string twit, TweetStat tweetStat)
        {
            var matches = _emojiRegex.Matches(twit);

            if (matches.Count == 0)
                return;

            foreach (Match match in matches)
            {
                tweetStat.Emojis.AddOrUpdate(match.Value, 1, (k, c) => { return c + 1; });
            }

            tweetStat.TweetCountWithEmojis++;
        }

        #endregion Processing elements of the twit
    }
}
