using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Processor.Client;
using Processor.Entities;
using Processor.Managers.Interfaces;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Processor.Managers
{
    public class TweetManager : ITweetManager
    {
        private static ConcurrentQueue<string> _queue;
        private static int _count;
        private static bool _isRunning;

        private static HttpClientWrapper _httpClient;
        private static TwitterServiceOptions _twitterService;
        private CancellationTokenSource cancellationSource = new CancellationTokenSource();

        public TweetManager(ConcurrentQueue<string> queue, IOptions<TwitterServiceOptions> _twitterServiceOptions, HttpClientWrapper httpClient)
        {
            _queue = queue;
            _twitterService = _twitterServiceOptions.Value;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Start Twitter stream
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            if (_isRunning)
                return;

            _isRunning = true;

            cancellationSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationSource.Token;

            var twits = ReceiveTwitsAsync(cancellationToken);
            await foreach (var twit in twits)
            {
                _queue.Enqueue(twit);
                _count++;
                Debug.WriteLine($"Enqueued Twit: {_count}");
            }

            Log.Information("TweetManager started");
        }


        /// <summary>
        /// Cancel Twitter stream
        /// </summary>
        public void Stop()
        {
            cancellationSource.Cancel();
            _isRunning = false;
            Log.Information("TweetManager stopped");
        }

        private async static IAsyncEnumerable<string> ReceiveTwitsAsync(CancellationToken cancellationToken)
        {
            var stream = await _httpClient.GetStreamAsync(_twitterService.ServiceUrl, cancellationToken);

            var result = ProcessStreamAsync(stream, cancellationToken);
            await foreach (var twit in result)
            {
                yield return twit;
            }
        }

        private async static IAsyncEnumerable<string> ProcessStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                jsonTextReader.SupportMultipleContent = true;

                while (await jsonTextReader.ReadAsync() && !cancellationToken.IsCancellationRequested)
                {
                    var tweet = JObject.Load(jsonTextReader);
                    var text = tweet.SelectToken("data.text").ToString();
                    yield return text;
                }
            }
        }
    }
}
