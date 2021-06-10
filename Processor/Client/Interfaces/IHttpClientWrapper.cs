using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Processor.Client.Interfaces
{
    public interface IHttpClientWrapper
    {
        Task<Stream> GetStreamAsync(string url, CancellationToken cancellationToken);
    }
}
