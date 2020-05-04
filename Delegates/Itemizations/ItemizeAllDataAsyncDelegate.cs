using System.Collections.Generic;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;

namespace Delegates.Itemizations
{
    // TODO: Deprecate as it's a trivial delegate, not adding value
    public class ItemizeAllDataAsyncDelegate<Type>: IItemizeAllAsyncDelegate<Type>
    {
        private readonly IGetDataAsyncDelegate<List<Type>, string> getDataCollectionAsyncDelegate;

        public ItemizeAllDataAsyncDelegate(
            IGetDataAsyncDelegate<List<Type>, string> getDataCollectionAsyncDelegate)
        {
            this.getDataCollectionAsyncDelegate = getDataCollectionAsyncDelegate;
        }

        public async IAsyncEnumerable<Type> ItemizeAllAsync()
        {
            var data = await getDataCollectionAsyncDelegate.GetDataAsync(string.Empty);
            foreach (var dataValue in data)
                yield return dataValue;
        }
    }
}