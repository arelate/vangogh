using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Recycle;
using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Index;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Records;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Stash;

using Interfaces.Status;

using Interfaces.Models.RecordsTypes;

namespace Controllers.Data
{
    public class DataController<Type> : IDataController<Type>
    {
        readonly IStashController<Dictionary<long, Type>> stashController;
        readonly IConvertDelegate<Type, long> convertProductToIndexDelegate;
        readonly IRecordsController<long> recordsController;
        readonly IStatusController statusController;
        readonly ICommitAsyncDelegate[] additionalCommitDelegates;

        public DataController(
            IStashController<Dictionary<long, Type>> stashController,
            IConvertDelegate<Type, long> convertProductToIndexDelegate,
            IRecordsController<long> recordsController,
            IStatusController statusController,
            params ICommitAsyncDelegate[] additionalCommitDelegates)
        {
            this.stashController = stashController;

            this.convertProductToIndexDelegate = convertProductToIndexDelegate;

            this.recordsController = recordsController;

            this.statusController = statusController;
            this.additionalCommitDelegates = additionalCommitDelegates;
        }

        public async Task<bool> ContainsAsync(Type item, IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            return data.ContainsValue(item);
        }

        public async Task<bool> ContainsIdAsync(long id, IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            return data.ContainsKey(id);
        }

        public async Task<Type> GetByIdAsync(long id, IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            if (data.ContainsKey(id)) return data[id];
            else return default(Type);
        }

        public async Task<int> CountAsync(IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            return data.Count;
        }

        public async Task UpdateAsync(Type updatedData, IStatus status)
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

        public async Task DeleteAsync(Type deletedData, IStatus status)
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

        public async Task<IEnumerable<long>> ItemizeAllAsync(IStatus status)
        {
            var data = await stashController.GetDataAsync(status);
            return data.Keys;
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

            var additionalCommitsTask = await statusController.CreateAsync(commitTask, "Additional commit dependencies");
            var current = 0;

            foreach (var commitDelegate in additionalCommitDelegates)
            {
                await statusController.UpdateProgressAsync(
                    additionalCommitsTask,
                    ++current,
                    additionalCommitDelegates.Length,
                    "Commit delegate");

                await commitDelegate.CommitAsync(additionalCommitsTask);
            }

            await statusController.CompleteAsync(additionalCommitsTask);

            await statusController.CompleteAsync(commitTask);
        }
    }
}
