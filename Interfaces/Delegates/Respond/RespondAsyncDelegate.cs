using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Models.Logs;

namespace Interfaces.Delegates.Respond
{
    public interface IRespondAsyncDelegate
    {
        Task<ISessionLog> RespondAsync(IDictionary<string, IEnumerable<string>> parameters);
    }
}