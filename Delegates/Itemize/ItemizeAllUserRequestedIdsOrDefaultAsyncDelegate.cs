using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Delegates.Itemize;
using Interfaces.Status;

namespace Delegates.EnumerateIds
{
    public class ItemizeAllUserRequestedIdsOrDefaultAsyncDelegate : IItemizeAllAsyncDelegate<long>
    {
        private IItemizeAllAsyncDelegate<long> itemizeUserRequestedIdsAsyncDelegate;
        private IItemizeAllAsyncDelegate<long>[] itemizeDefaultIdsDelegates;

        public ItemizeAllUserRequestedIdsOrDefaultAsyncDelegate(
            IItemizeAllAsyncDelegate<long> itemizeUserRequestedIdsAsyncDelegate,
            params IItemizeAllAsyncDelegate<long>[] itemizeDefaultIdsDelegates)
        {
            this.itemizeUserRequestedIdsAsyncDelegate = itemizeUserRequestedIdsAsyncDelegate;
            this.itemizeDefaultIdsDelegates = itemizeDefaultIdsDelegates;
        }

        public async Task<IEnumerable<long>> ItemizeAllAsync(IStatus status)
        {
            var userRequestedIds = await itemizeUserRequestedIdsAsyncDelegate.ItemizeAllAsync(status);

            if (userRequestedIds != null || userRequestedIds.Count() > 0) return userRequestedIds;

            var otherIds = new List<long>();
            foreach (var enumerableDelegate in itemizeDefaultIdsDelegates)
                otherIds.AddRange(await enumerableDelegate.ItemizeAllAsync(status));

            return otherIds;
        }
    }
}
