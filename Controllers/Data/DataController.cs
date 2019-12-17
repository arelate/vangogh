using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Interfaces.Models.RecordsTypes;

namespace Controllers.Data
{
    public abstract class DataController<DataType> : IDataController<DataType>
    {
        readonly IStashController<List<DataType>> stashController;
        readonly IConvertDelegate<DataType, long> convertProductToIndexDelegate;
        readonly IRecordsController<long> recordsController;
        readonly private ICollectionController collectionController;
        readonly IStatusController statusController;

        public DataController(
            IStashController<List<DataType>> stashController,
            IConvertDelegate<DataType, long> convertProductToIndexDelegate,
            IRecordsController<long> recordsController,
            ICollectionController collectionController,
            IStatusController statusController)
        {
            this.stashController = stashController;
            this.convertProductToIndexDelegate = convertProductToIndexDelegate;
            this.recordsController = recordsController;
            this.collectionController = collectionController;
            this.statusController = statusController;
        }

        public async Task<bool> ContainsAsync(DataType item, IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            return data.Contains(item);
        }

        public async Task<bool> ContainsIdAsync(long id, IStatus status)
        {
            return await GetByIdAsync(id, status) != null;
        }

        public async Task<DataType> GetByIdAsync(long id, IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            return collectionController.Find(data, item =>
            {
                var index = convertProductToIndexDelegate.Convert(item);
                return index == id;
            });
        }

        public async Task<int> CountAsync(IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            return data.Count;
        }

        public async Task UpdateAsync(DataType updatedData, IStatus status)
        {
            var updatedDataId = convertProductToIndexDelegate.Convert(updatedData);
            var recordType = RecordsTypes.Created;

            if (await ContainsIdAsync(updatedDataId, status))
            {
                await DeleteAsync(updatedData, status);
                recordType = RecordsTypes.Updated;
            }
            
            var data = await stashController.GetDataAsync(status);
            data.Add(updatedData);

            if (recordsController != null)
                await recordsController.SetRecordAsync(
                    updatedDataId,
                    recordType,
                    status);
        }

        public async Task DeleteAsync(DataType deletedData, IStatus status)
        {
            var index = convertProductToIndexDelegate.Convert(deletedData);

            if (!await ContainsIdAsync(index, status)) return;

            var data = await stashController.GetDataAsync(status);
            data.Remove(deletedData);

            if (recordsController != null)
                await recordsController.SetRecordAsync(
                    index,
                    RecordsTypes.Deleted,
                    status);
        }

        public async IAsyncEnumerable<DataType> ItemizeAllAsync(IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            foreach (var dataValue in data)
                yield return dataValue;
        }

        public async Task CommitAsync(IStatus status)
        {
            var commitTask = await statusController.CreateAsync(status, "Commit updated data");

            // commit records controller
            if (recordsController != null)
            {
                var commitRecordsTask = await statusController.CreateAsync(commitTask, "Commit records");
                await recordsController.CommitAsync(status);
                await statusController.CompleteAsync(commitRecordsTask);
            }

            var commitDataTask = await statusController.CreateAsync(commitTask, "Commit items");
            await stashController.SaveAsync(commitDataTask);
            await statusController.CompleteAsync(commitDataTask);

            await statusController.CompleteAsync(commitTask);
        }
    }
}
