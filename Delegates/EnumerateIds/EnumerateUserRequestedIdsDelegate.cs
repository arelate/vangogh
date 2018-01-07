using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;

using Interfaces.Status;

namespace Delegates.EnumerateIds
{
    public class EnumerateUserRequestedIdsDelegate : IEnumerateIdsAsyncDelegate
    {
        private string[] userRequested;

        public EnumerateUserRequestedIdsDelegate(params string[] userRequested)
        {
            this.userRequested = userRequested;
        }

        public async Task<IEnumerable<long>> EnumerateIdsAsync(IStatus status)
        {
            var parsedIds = new List<long>();

            await Task.Run(() =>
            {
                if (userRequested != null && userRequested.Length > 0)
                {

                    foreach (var stringId in userRequested)
                        if (long.TryParse(stringId, out var id))
                            parsedIds.Add(id);
                }
            });

            return parsedIds;
        }
    }
}
