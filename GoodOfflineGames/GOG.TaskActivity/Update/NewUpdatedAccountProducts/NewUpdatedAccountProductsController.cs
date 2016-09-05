using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Storage;
using Interfaces.Reporting;
using Interfaces.ProductTypes;

using GOG.TaskActivities.Abstract;
using GOG.Models;

namespace GOG.TaskActivities.Update.NewUpdatedAccountProducts
{
    public class NewUpdatedAccountProductsController: TaskActivityController
    {
        private IProductTypeStorageController productStorageController;

        public NewUpdatedAccountProductsController(
            IProductTypeStorageController productStorageController,
            ITaskReportingController taskReportingController): base(taskReportingController)
        {
            this.productStorageController = productStorageController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask("Load existing new or updated products");
            var newUpdatedProducts = await productStorageController.Pull<long>(ProductTypes.NewUpdatedProduct);

            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Load account products");
            var accountProducts = await productStorageController.Pull<AccountProduct>(ProductTypes.AccountProduct);
            taskReportingController.CompleteTask();

            if (accountProducts == null) return;

            taskReportingController.StartTask("Update new or updated account products");

            foreach (var product in accountProducts)
            {
                if (product == null) continue;
                if (product.IsNew && !newUpdatedProducts.Contains(product.Id)) newUpdatedProducts.Add(product.Id);
                if (product.Updates > 0 && !newUpdatedProducts.Contains(product.Id)) newUpdatedProducts.Add(product.Id);
            }

            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Save new or updated products");
            await productStorageController.Push(ProductTypes.NewUpdatedProduct, newUpdatedProducts);
            taskReportingController.CompleteTask();
        }
    }
}
