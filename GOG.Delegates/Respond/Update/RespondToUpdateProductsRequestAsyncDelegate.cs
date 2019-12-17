using System.Threading.Tasks;
using System.Collections.Generic;

using Interfaces.Delegates.Respond;

using Attributes;

namespace GOG.Delegates.Respond.Update
{
    [RespondsToRequests(Method = "update", Collection = "products")]
    public class RespondToUpdateProductsRequestAsyncDelegate : IRespondAsyncDelegate
    {
        public Task RespondAsync(IDictionary<string, IEnumerable<string>> parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}