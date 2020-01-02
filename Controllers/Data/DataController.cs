using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Logs;

using Interfaces.Delegates.Convert;

using Interfaces.Models.RecordsTypes;

namespace Controllers.Data
{
    public abstract class DataController<DataType> : IDataController<DataType>
    {
        readonly IStashController<List<DataType>> stashController;
        readonly IConvertDelegate<DataType, long> convertProductToIndexDelegate;
        readonly IRecordsController<long> recordsController;
        readonly private ICollectionController collectionController;
        readonly IActionLogController actionLogController;

        public DataController(
            IStashController<List<DataType>> stashController,
            IConvertDelegate<DataType, long> convertProductToIndexDelegate,
            IRecordsController<long> recordsController,
            ICollectionController collectionController,
            IActionLogController actionLogController)
        {
            this.stashController = stashController;
            this.convertProductToIndexDelegate = convertProductToIndexDelegate;
            this.recordsController = recordsController;
            this.collectionController = collectionController;
            this.actionLogController = actionLogController;
        }

        public async Task<bool> ContainsAsync(DataType item)
        {
            var data = await stashController.GetDataAsync();
            return data.Contains(item);
        }

        public async Task<bool> ContainsIdAsync(long id)
        {
            return await GetByIdAsync(id) != null;
        }

        public async Task<DataType> GetByIdAsync(long id)
        {
            var data = await stashController.GetDataAsync();
            return collectionController.Find(data, item =>
            {
                var index = convertProductToIndexDelegate.Convert(item);
                return index == id;
            });
        }

        public async Task<int> CountAsync()
        {
            var data = await stashController.GetDataAsync();
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
            
            var data = await stashController.GetDataAsync();
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

            var data = await stashController.GetDataAsync();
            data.Remove(deletedData);

            if (recordsController != null)
                await recordsController.SetRecordAsync(
                    index,
                    RecordsTypes.Deleted);
        }

        public async IAsyncEnumerable<DataType> ItemizeAllAsync()
        {
            var data = await stashController.GetDataAsync();
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
            await stashController.SaveAsync();
            actionLogController.CompleteAction();

            actionLogController.CompleteAction();
        }
    }
}
