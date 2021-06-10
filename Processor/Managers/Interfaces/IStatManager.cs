using Processor.Entities;
using System.Threading.Tasks;

namespace Processor.Managers.Interfaces
{
    public interface IStatisticsManager
    {
        Task<TweetStatUi> GetStatisticsAsync();
        void StartTime();
        void StopTime();
    }
}