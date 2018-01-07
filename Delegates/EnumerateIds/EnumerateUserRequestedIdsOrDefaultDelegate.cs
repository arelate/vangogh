using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Controllers.Data;
using Interfaces.Status;

namespace Delegates.EnumerateIds
{
    // TODO: itemization
    public class EnumerateUserRequestedIdsOrDefaultDelegate : IEnumerateIdsAsyncDelegate
    {
        private IEnumerateIdsAsyncDelegate enumerateUserRequestedIdsAsyncDelegate;
        private IEnumerateIdsAsyncDelegate[] otherEnumerateDelegates;

        public EnumerateUserRequestedIdsOrDefaultDelegate(
            IEnumerateIdsAsyncDelegate enumerateUserRequestedIdsAsyncDelegate,
            params IEnumerateIdsAsyncDelegate[] otherEnumerateDelegates)
        {
            this.enumerateUserRequestedIdsAsyncDelegate = enumerateUserRequestedIdsAsyncDelegate;
            this.otherEnumerateDelegates = otherEnumerateDelegates;
        }

        public async Task<IEnumerable<long>> EnumerateIdsAsync(IStatus status)
        {
            var userRequestedIds = await enumerateUserRequestedIdsAsyncDelegate.EnumerateIdsAsync(status);

            if (userRequestedIds != null || userRequestedIds.Count() > 0) return userRequestedIds;

            var otherIds = new List<long>();
            foreach (var enumerableDelegate in otherEnumerateDelegates)
                otherIds.AddRange(await enumerableDelegate.EnumerateIdsAsync(status));

            return otherIds;
        }
    }
}
