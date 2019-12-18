using System.Collections.Generic;
using System.Threading.Tasks;

namespace GOG.Interfaces.Delegates.GetDeserialized
{
    public interface IGetDeserializedAsyncDelegate<T>
    {
        Task<T> GetDeserializedAsync(string uri, IDictionary<string, string> parameters = null);
    }
}
