using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Collection;
using Interfaces.Data;
using Interfaces.TaskStatus;

using GOG.Models;

namespace GOG.Controllers.Collection
{
    public class NewUpdatedCollectionProcessingController : ICollectionProcessingController<AccountProduct>
    {
        private ICollectionController collectionController;
        private IDataController<long> updatedDataController;
        private IDataController<long> lastKnownValidDataController;
        private ITaskStatusController taskStatusController;

        public NewUpdatedCollectionProcessingController(
            ICollectionController collectionController,
            IDataController<long> updatedDataController,
            IDataController<long> lastKnownValidDataController,
            ITaskStatusController taskStatusController)
        {
            this.collectionController = collectionController;
            this.updatedDataController = updatedDataController;
            this.lastKnownValidDataController = lastKnownValidDataController;
            this.taskStatusController = taskStatusController;
        }

        public async Task Process(IEnumerable<AccountProduct> accountProducts, ITaskStatus taskStatus)
        {
            var extractNewUpdatedTask = taskStatusController.Create(taskStatus, "Extract new and updated products");

            var newUpdatedAccountProducts = collectionController.Reduce(accountProducts,
                ap =>
                {
                    return ap.IsNew || ap.Updates > 0;
                });

            var updatedIds = newUpdatedAccountProducts.Select(ap => ap.Id).ToArray();

            await updatedDataController.UpdateAsync(extractNewUpdatedTask, updatedIds);

            await lastKnownValidDataController.RemoveAsync(extractNewUpdatedTask, updatedIds);

            taskStatusController.Complete(extractNewUpdatedTask);
        }
    }
}
