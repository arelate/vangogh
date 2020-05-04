using System.Threading.Tasks;
using System.Collections.Generic;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Conversions;

namespace Delegates.Data.Models
{
    public class DeleteAsyncDelegate<Type> : IDeleteAsyncDelegate<Type>
    {
        private readonly IGetDataAsyncDelegate<List<Type>, string> getDataCollectionAsyncDelegate;
        private readonly IConvertDelegate<Type, long> convertProductToIndexDelegate;
        private readonly IConfirmAsyncDelegate<long> confirmContainsAsyncDelegate;

        public DeleteAsyncDelegate(
            IGetDataAsyncDelegate<List<Type>, string> getDataCollectionAsyncDelegate,
            IConvertDelegate<Type, long> convertProductToIndexDelegate,
            IConfirmAsyncDelegate<long> confirmContainsAsyncDelegate)
        {
            this.getDataCollectionAsyncDelegate = getDataCollectionAsyncDelegate;
            this.convertProductToIndexDelegate = convertProductToIndexDelegate;
            this.confirmContainsAsyncDelegate = confirmContainsAsyncDelegate;
        }

        public async Task DeleteAsync(Type deletedData)
        {
            var index = convertProductToIndexDelegate.Convert(deletedData);
            
            if (!await confirmContainsAsyncDelegate.ConfirmAsync(index)) return;
            
            var data = await getDataCollectionAsyncDelegate.GetDataAsync(string.Empty);
            data.Remove(deletedData);
            
            //
            //     if (recordsController != null)
            //         await recordsController.SetRecordAsync(
            //             index,
            //             RecordsTypes.Deleted);
        }
    }
}