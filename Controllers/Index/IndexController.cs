using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Index;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Stash;

using Interfaces.Status;

namespace Controllers.Index
{
    public class IndexController<Type> : IIndexController<Type>
    {
        private IStashController<List<Type>> indexesStashController;

        private ICollectionController collectionController;

        private IStatusController statusController;

        public IndexController(
            IStashController<List<Type>> indexesStashController,
            ICollectionController collectionController,
            IStatusController statusController)
        {
            this.indexesStashController = indexesStashController;

            this.collectionController = collectionController;

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

        private async Task Map(IStatus status, string taskMessage, Func<Type, bool> itemAction, params Type[] data)
        {
            var task = await statusController.CreateAsync(status, taskMessage);
            var counter = 0;
            var dataChanged = false;

            foreach (var item in data)
            {
                await statusController.UpdateProgressAsync(
                    task,
                    ++counter,
                    data.Length,
                    item.ToString());

                // do this for every item
                if (itemAction(item)) dataChanged = true;
            }

            if (dataChanged)
            {
                var saveDataTask = await statusController.CreateAsync(task, "Save modified index");
                await indexesStashController.SaveAsync(status);
                await statusController.CompleteAsync(saveDataTask);
            }

            await statusController.CompleteAsync(task);
        }

        public async Task DeleteAsync(IStatus status, params Type[] data)
        {
            var indexes = await indexesStashController.GetDataAsync(status);

            await Map(
                status,
                "Delete index item(s)",
                (item) =>
                {
                    if (indexes.Contains(item))
                    {
                        indexes.Remove(item);
                        return true;
                    }
                    return false;
                },
                data);
        }

        public async Task CreateAsync(IStatus status, params Type[] data)
        {
            var indexes = await indexesStashController.GetDataAsync(status);

            await Map(
                status,
                "Create index item(s)",
                (item) => {
                    if (!indexes.Contains(item))
                    {
                        indexes.Add(item);
                        return true;
                    }
                    return false;
                },
                data);
        }
    }
}
