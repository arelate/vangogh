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
        private IStashController<List<Type>> indexesStashController;

        private ICollectionController collectionController;

        private IRecordsController<Type> recordsController;

        private IStatusController statusController;

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

        private async Task Map(IStatus status, string taskMessage, Func<Type, Task<bool>> itemAction, Type data)
        {
            var task = await statusController.CreateAsync(status, taskMessage);

            if (await itemAction(data)) 
            {
                var saveDataTask = await statusController.CreateAsync(task, "Save modified index");
                await indexesStashController.SaveAsync(status);
                await statusController.CompleteAsync(saveDataTask);
            }

            await statusController.CompleteAsync(task);
        }

        public async Task DeleteAsync(Type data, IStatus status)
        {
            var indexes = await indexesStashController.GetDataAsync(status);

            await Map(
                status,
                "Delete index item(s)",
                async (item) =>
                {
                    if (indexes.Contains(item))
                    {
                        indexes.Remove(item);

                    await recordsController.SetRecordAsync(
                        item,
                        RecordsTypes.Removed,
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

            await Map(
                status,
                "Create index item(s)",
                async (item) => {
                    if (!indexes.Contains(item))
                    {
                        indexes.Add(item);

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
    }
}
