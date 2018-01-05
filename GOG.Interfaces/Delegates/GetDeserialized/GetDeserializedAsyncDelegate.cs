using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Status;

namespace GOG.Interfaces.Delegates.GetDeserialized
{
    public interface IGetDeserializedAsyncDelegate<T>
    {
        Task<T> GetDeserializedAsync(IStatus status, string uri, IDictionary<string, string> parameters = null);
    }
}
