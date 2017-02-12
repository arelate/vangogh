using System.Threading.Tasks;
using System.Linq;

using Interfaces.TaskStatus;
using Interfaces.Data;

using GOG.Models;

namespace GOG.TaskActivities.Update.NewUpdatedAccountProducts
{
    public class NewUpdatedAccountProductsController: TaskActivityController
    {
        private IDataController<long> updatedDataController;
        private IDataController<long> lastKnownValidDataController;
        private IDataController<AccountProduct> accountProductsDataController;

        public NewUpdatedAccountProductsController(
            IDataController<long> updatedDataController,
            IDataController<long> lastKnownValidDataController,
            IDataController<AccountProduct> accountProductsDataController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController): 
            base(
                taskStatus,
                taskStatusController)
        {
            this.updatedDataController = updatedDataController;
            this.lastKnownValidDataController = lastKnownValidDataController;
            this.accountProductsDataController = accountProductsDataController;
        }

        public override async Task ProcessTaskAsync()
        {
            var getUpdatedTask = taskStatusController.Create(taskStatus, "Get new or updated account products");

            var accountProductIds = accountProductsDataController.EnumerateIds().ToArray();
            var counter = 0;

            foreach (var id in accountProductIds)
            {
                taskStatusController.UpdateProgress(
                    getUpdatedTask,
                    ++counter,
                    accountProductIds.Length,
                    id.ToString());

                if (updatedDataController.Contains(id)) continue;

                var accountProduct = await accountProductsDataController.GetByIdAsync(id);

                if (accountProduct.IsNew ||
                    accountProduct.Updates > 0)
                {
                    await updatedDataController.UpdateAsync(getUpdatedTask, id);

                    // since we known the product was updated - remove last known valid state
                    await lastKnownValidDataController.RemoveAsync(getUpdatedTask, id);
                }
            }

            taskStatusController.Complete(getUpdatedTask);
        }
    }
}
