using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Conversions;

namespace Delegates.Data.Models
{
    public abstract class GetDataByIdAsyncDelegate<Type> : IGetDataAsyncDelegate<Type, long>
    {
        private readonly IGetDataAsyncDelegate<List<Type>, string> getDataCollectionAsyncDelegate;
        private readonly IFindDelegate<Type> findDelegate;
        private readonly IConvertDelegate<Type, long> convertProductToIndexDelegate;

        protected GetDataByIdAsyncDelegate(
            IGetDataAsyncDelegate<List<Type>, string> getDataCollectionAsyncDelegate,
            IFindDelegate<Type> findDelegate,
            IConvertDelegate<Type, long> convertProductToIndexDelegate)
        {
            this.getDataCollectionAsyncDelegate = getDataCollectionAsyncDelegate;
            this.findDelegate = findDelegate;
            this.convertProductToIndexDelegate = convertProductToIndexDelegate;
        }

        public async Task<Type> GetDataAsync(long id)
        {
            var dataCollection = await getDataCollectionAsyncDelegate.GetDataAsync(string.Empty);
            return findDelegate.Find(dataCollection, item =>
            {
                var index = convertProductToIndexDelegate.Convert(item);
                return index == id;
            });
        }
    };
}