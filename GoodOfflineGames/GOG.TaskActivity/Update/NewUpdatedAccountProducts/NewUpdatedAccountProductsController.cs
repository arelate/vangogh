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
        private IDataController<AccountProduct> accountProductsDataController;

        public NewUpdatedAccountProductsController(
            IDataController<long> updatedDataController,
            IDataController<AccountProduct> accountProductsDataController,
            ITaskReportingController taskReportingController): base(taskReportingController)
        {
            this.updatedDataController = updatedDataController;
            this.accountProductsDataController = accountProductsDataController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask("Process new or updated account products");

            foreach (var id in accountProductsDataController.EnumerateIds())
            {
                if (updatedDataController.Contains(id)) continue;

                var accountProduct = await accountProductsDataController.GetById(id);

                if (accountProduct.IsNew ||
                    accountProduct.Updates > 0)
                    await updatedDataController.Update(id);
            }

            taskReportingController.CompleteTask();
        }
    }
}
