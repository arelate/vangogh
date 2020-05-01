using System.Threading.Tasks;

namespace Interfaces.Delegates.Data
{
    public interface IGetDataAsyncDelegate<Type, in UriType>
    {
        Task<Type> GetDataAsync(UriType uri);
    }
}