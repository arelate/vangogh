using System.Threading.Tasks;

namespace Interfaces.Delegates.Data
{
    public interface IGetDataToDestinationAsyncDelegate<in SourceType, in DestinationType>
    {
        Task GetDataToDestinationAsyncDelegate(SourceType source, DestinationType destination);
    }
}