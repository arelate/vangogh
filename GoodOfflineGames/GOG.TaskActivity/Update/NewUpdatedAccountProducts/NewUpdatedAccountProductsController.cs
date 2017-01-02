using System.Threading.Tasks;
using Interfaces.Reporting;
using Interfaces.Data;

using GOG.TaskActivities.Abstract;
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
            ITaskReportingController taskReportingController): base(taskReportingController)
        {
            this.updatedDataController = updatedDataController;
            this.lastKnownValidDataController = lastKnownValidDataController;
            this.accountProductsDataController = accountProductsDataController;
        }

        public override async Task ProcessTaskAsync()
        {
            taskReportingController.StartTask("Process new or updated account products");

            foreach (var id in accountProductsDataController.EnumerateIds())
            {
                if (updatedDataController.Contains(id)) continue;

                var accountProduct = await accountProductsDataController.GetByIdAsync(id);

                if (accountProduct.IsNew ||
                    accountProduct.Updates > 0)
                {
                    await updatedDataController.UpdateAsync(id);

                    // since we known the product was updated - remove last known valid state
                    await lastKnownValidDataController.RemoveAsync(id);
                }
            }

            taskReportingController.CompleteTask();
        }
    }
}
