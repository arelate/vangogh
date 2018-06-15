using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Index;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Records;

using Interfaces.Status;

using Interfaces.Models.RecordsTypes;

namespace Controllers.Index
{
    public class IndexController<Type> : IIndexController<Type>
    {
        readonly IStashController<List<Type>> indexesStashController;

        ICollectionController collectionController;

        readonly IRecordsController<Type> recordsController;

        IStatusController statusController;

        public IndexController(
            IStashController<List<Type>> indexesStashController,
            ICollectionController collectionController,
            IRecordsController<Type> recordsController,
            IStatusController statusController)
        {
            this.indexesStashController = indexesStashController;
            this.collectionController = collectionController;
            this.recordsController = recordsController;

            this.statusController = statusController;
        }

        public async Task<bool> ContainsIdAsync(Type id, IStatus status)
        {
            var indexes = await indexesStashController.GetDataAsync(status);
            return indexes.Contains(id);
        }

        public async Task<int> CountAsync(IStatus status)
        {
            var indexes = await indexesStashController.GetDataAsync(status);
            return indexes.Count;
        }

        public async Task<IEnumerable<Type>> ItemizeAllAsync(IStatus status)
        {
            return await indexesStashController.GetDataAsync(status);
        }

        async Task MapAsync(IStatus status, string taskMessage, Func<Type, Task<bool>> itemAction, Type data)
        {
            var task = await statusController.CreateAsync(status, taskMessage, false);

            if (await itemAction(data))
            {
                //var saveDataTask = await statusController.CreateAsync(task, "Save modified index", false);
                //await indexesStashController.SaveAsync(status);
                //await statusController.CompleteAsync(saveDataTask, false);
            }

            await statusController.CompleteAsync(task, false);
        }

        public async Task DeleteAsync(Type data, IStatus status)
        {
            var indexes = await indexesStashController.GetDataAsync(status);

            await MapAsync(
                status,
                "Delete index item(s)",
                async (item) =>
                {
                    if (indexes.Contains(item))
                    {
                        indexes.Remove(item);

                        if (recordsController != null)
                            await recordsController.SetRecordAsync(
                                item,
                                RecordsTypes.Deleted,
                                status);

                        return true;
                    }
                    return false;
                },
                data);
        }

        public async Task CreateAsync(Type data, IStatus status)
        {
            var indexes = await indexesStashController.GetDataAsync(status);

            await MapAsync(
                status,
                "Create index item(s)",
                async (item) => {
                    if (!indexes.Contains(item))
                    {
                        indexes.Add(item);

                        if (recordsController != null)
                            await recordsController.SetRecordAsync(
                                item,
                                RecordsTypes.Created,
                                status);

                        return true;
                    }
                    return false;
                },
                data);
        }

        public async Task CommitAsync(IStatus status)
        {
            if (indexesStashController.DataAvailable)
            {
                var saveDataTask = await statusController.CreateAsync(status, "Save modified index", false);
                await indexesStashController.SaveAsync(status);
                await statusController.CompleteAsync(saveDataTask, false);
            }
        }
    }
}
