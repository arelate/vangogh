using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Find;
using Interfaces.Delegates.GetData;
using Interfaces.Delegates.PostData;

using Interfaces.Models.RecordsTypes;

namespace Controllers.Data
{
    public abstract class DataController<DataType> : IDataController<DataType>
    {
        private readonly IGetDataAsyncDelegate<List<DataType>> getDataAsyncDelegate;
        private readonly IPostDataAsyncDelegate<List<DataType>> postDataAsyncDelegate;
        readonly IConvertDelegate<DataType, long> convertProductToIndexDelegate;
        readonly IRecordsController<long> recordsController;
        readonly private IFindDelegate<DataType> findDelegate;
        readonly IActionLogController actionLogController;

        public DataController(
            IGetDataAsyncDelegate<List<DataType>> getDataAsyncDelegate,
            IPostDataAsyncDelegate<List<DataType>> postDataAsyncDelegate,
            IConvertDelegate<DataType, long> convertProductToIndexDelegate,
            IRecordsController<long> recordsController,
            IFindDelegate<DataType> findDelegate,
            IActionLogController actionLogController)
        {
            this.getDataAsyncDelegate = getDataAsyncDelegate;
            this.postDataAsyncDelegate = postDataAsyncDelegate;
            this.convertProductToIndexDelegate = convertProductToIndexDelegate;
            this.recordsController = recordsController;
            this.findDelegate = findDelegate;
            this.actionLogController = actionLogController;
        }

        public async Task<bool> ContainsAsync(DataType item)
        {
            var data = await getDataAsyncDelegate.GetDataAsync();
            return data.Contains(item);
        }

        public async Task<bool> ContainsIdAsync(long id)
        {
            return await GetByIdAsync(id) != null;
        }

        public async Task<DataType> GetByIdAsync(long id)
        {
            var data = await getDataAsyncDelegate.GetDataAsync();
            return findDelegate.Find(data, item =>
            {
                var index = convertProductToIndexDelegate.Convert(item);
                return index == id;
            });
        }

        public async Task<int> CountAsync()
        {
            var data = await getDataAsyncDelegate.GetDataAsync();
            return data.Count;
        }

        public async Task UpdateAsync(DataType updatedData)
        {
            var updatedDataId = convertProductToIndexDelegate.Convert(updatedData);
            var recordType = RecordsTypes.Created;

            if (await ContainsIdAsync(updatedDataId))
            {
                await DeleteAsync(updatedData);
                recordType = RecordsTypes.Updated;
            }
            
            var data = await getDataAsyncDelegate.GetDataAsync();
            data.Add(updatedData);

            if (recordsController != null)
                await recordsController.SetRecordAsync(
                    updatedDataId,
                    recordType);
        }

        public async Task DeleteAsync(DataType deletedData)
        {
            var index = convertProductToIndexDelegate.Convert(deletedData);

            if (!await ContainsIdAsync(index)) return;

            var data = await getDataAsyncDelegate.GetDataAsync();
            data.Remove(deletedData);

            if (recordsController != null)
                await recordsController.SetRecordAsync(
                    index,
                    RecordsTypes.Deleted);
        }

        public async IAsyncEnumerable<DataType> ItemizeAllAsync()
        {
            var data = await getDataAsyncDelegate.GetDataAsync();
            foreach (var dataValue in data)
                yield return dataValue;
        }

        public async Task CommitAsync()
        {
            actionLogController.StartAction("Commit updated data");

            // commit records controller
            if (recordsController != null)
            {
                actionLogController.StartAction("Commit records");
                await recordsController.CommitAsync();
                actionLogController.CompleteAction();
            }

            actionLogController.StartAction("Commit items");
            var data = await getDataAsyncDelegate.GetDataAsync();
            await postDataAsyncDelegate.PostDataAsync(data);
            actionLogController.CompleteAction();

            actionLogController.CompleteAction();
        }
    }
}
