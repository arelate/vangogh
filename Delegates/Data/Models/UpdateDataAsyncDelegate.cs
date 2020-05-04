using System.Threading.Tasks;
using System.Collections.Generic;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Interfaces.Models.RecordTypes;
using Interfaces.Delegates.Server;

namespace Delegates.Data.Models
{
    public class UpdateDataAsyncDelegate<Type>: IUpdateAsyncDelegate<Type>
    {
        private readonly IDeleteAsyncDelegate<Type> deleteAsyncDelegate;
        private readonly IConvertDelegate<Type, long> convertProductToIndexDelegate;
        private readonly IConfirmAsyncDelegate<long> confirmDataContainsIdAsyncDelegate;
        private readonly IGetDataAsyncDelegate<List<Type>,string> getDataCollectionAsyncDelegate;

        public UpdateDataAsyncDelegate(
            IDeleteAsyncDelegate<Type> deleteAsyncDelegate,
            IConvertDelegate<Type, long> convertProductToIndexDelegate,
            IConfirmAsyncDelegate<long> confirmDataContainsIdAsyncDelegate,
            IGetDataAsyncDelegate<List<Type>,string> getDataCollectionAsyncDelegate)
        {
            this.deleteAsyncDelegate = deleteAsyncDelegate;
            this.convertProductToIndexDelegate = convertProductToIndexDelegate;
            this.confirmDataContainsIdAsyncDelegate = confirmDataContainsIdAsyncDelegate;
            this.getDataCollectionAsyncDelegate = getDataCollectionAsyncDelegate;
        }
        
        public async Task UpdateAsync(Type updatedData)
        {
            var updatedDataId = convertProductToIndexDelegate.Convert(updatedData);
            var recordType = RecordsTypes.Created;

            if (await confirmDataContainsIdAsyncDelegate.ConfirmAsync(updatedDataId))
            {
                await deleteAsyncDelegate.DeleteAsync(updatedData);
                recordType = RecordsTypes.Updated;
            }

            var data = await getDataCollectionAsyncDelegate.GetDataAsync(string.Empty);
            data.Add(updatedData);

            // if (recordsController != null)
            //     await recordsController.SetRecordAsync(
            //         updatedDataId,
            //         recordType);
        }
    }
}