using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Storage;
using Interfaces.Reporting;
using Interfaces.ProductTypes;

using GOG.TaskActivities.Abstract;
using GOG.Models;

namespace GOG.TaskActivities.UpdateNewUpdatedAccountProduct
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
            taskReportingController.AddTask("Load existing new or updated products");
            var newUpdatedProducts = await productStorageController.Pull<long>(ProductTypes.NewUpdatedProduct);
            if (newUpdatedProducts == null) newUpdatedProducts = new List<long>();
            taskReportingController.CompleteTask();

            taskReportingController.AddTask("Load account products");
            var accountProducts = await productStorageController.Pull<AccountProduct>(ProductTypes.AccountProduct);
            taskReportingController.CompleteTask();

            if (accountProducts == null) return;

            taskReportingController.AddTask("Update new or updated account products");

            foreach (var product in accountProducts)
            {
                if (product == null) continue;
                if (product.IsNew && !newUpdatedProducts.Contains(product.Id)) newUpdatedProducts.Add(product.Id);
                if (product.Updates > 0 && !newUpdatedProducts.Contains(product.Id)) newUpdatedProducts.Add(product.Id);
            }

            taskReportingController.CompleteTask();

            taskReportingController.AddTask("Save new or updated products");
            await productStorageController.Push(ProductTypes.NewUpdatedProduct, newUpdatedProducts);
            taskReportingController.CompleteTask();
        }
    }
}
