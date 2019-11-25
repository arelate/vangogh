using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.Stash;

using Interfaces.Status;

using Interfaces.Models.RecordsTypes;

namespace Controllers.Data
{
    public class DataController<DataType> : IDataController<DataType>
    {
        readonly IStashController<Dictionary<long, DataType>> stashController;
        readonly IConvertDelegate<DataType, long> convertProductToIndexDelegate;
        readonly IRecordsController<long> recordsController;
        readonly IStatusController statusController;

        public DataController(
            IStashController<Dictionary<long, DataType>> stashController,
            IConvertDelegate<DataType, long> convertProductToIndexDelegate,
            IRecordsController<long> recordsController,
            IStatusController statusController)
        {
            this.stashController = stashController;

            this.convertProductToIndexDelegate = convertProductToIndexDelegate;

            this.recordsController = recordsController;

            this.statusController = statusController;
        }

        public async Task<bool> ContainsAsync(DataType item, IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            return data.ContainsValue(item);
        }

        public async Task<bool> ContainsIdAsync(long id, IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            return data.ContainsKey(id);
        }

        public async Task<DataType> GetByIdAsync(long id, IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            if (data.ContainsKey(id)) return data[id];
            else return default(DataType);
        }

        public async Task<int> CountAsync(IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            return data.Count;
        }

        public async Task UpdateAsync(DataType updatedData, IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            var index = convertProductToIndexDelegate.Convert(updatedData);

            if (!data.ContainsKey(index))
            {
                data.Add(index, updatedData);

                if (recordsController != null)
                    await recordsController.SetRecordAsync(
                        index,
                        RecordsTypes.Created,
                        status);
            }
            else
            {
                data[index] = updatedData;

                if (recordsController != null)
                    await recordsController.SetRecordAsync(
                        index,
                        RecordsTypes.Updated,
                        status);
            }
        }

        public async Task DeleteAsync(DataType deletedData, IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            var index = convertProductToIndexDelegate.Convert(deletedData);

            if (data.ContainsKey(index))
            {
                data.Remove(index);

                if (recordsController != null)
                    await recordsController.SetRecordAsync(
                        index,
                        RecordsTypes.Deleted,
                        status);
            }
        }

        public async IAsyncEnumerable<DataType> ItemizeAllAsync(IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            foreach (var dataValue in data.Values)
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
