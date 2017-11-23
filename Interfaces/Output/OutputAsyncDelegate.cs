using System.Threading.Tasks;

namespace Interfaces.Output
{
    public interface IOutputContinuousAsyncDelegate<T>
    {
        Task OutputContinuousAsync(T data);
    }

}
