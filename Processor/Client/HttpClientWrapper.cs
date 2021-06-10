using Processor.Client.Interfaces;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Processor.Client
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient HttpClient;
        public HttpClientWrapper(string bearerToken)
        {
            if(string.IsNullOrEmpty(bearerToken))
            {
                throw new ArgumentException("Bearer token is not provided.");
            }

            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        }
        public async virtual Task<Stream> GetStreamAsync(string url, CancellationToken cancellationToken)
        {
            return await HttpClient.GetStreamAsync(url);
        }
    }
}
