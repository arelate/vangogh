using System.Threading.Tasks;
using System.Collections.Generic;

namespace Interfaces.Delegates.Respond
{
    public interface IRespondAsyncDelegate
    {
        Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters);
    }
}