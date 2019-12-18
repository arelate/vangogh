using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Models.Logs;

namespace Interfaces.Delegates.Respond
{
    public interface IRespondAsyncDelegate
    {
        Task<IResponseLog> RespondAsync(IDictionary<string, IEnumerable<string>> parameters);
    }
}