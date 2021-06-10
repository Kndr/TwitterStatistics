using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Processor.Managers.Interfaces
{
    public interface ITweetManager
    {
        Task StartAsync();
        void Stop();
    }
}