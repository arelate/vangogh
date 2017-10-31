using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Status;

using GOG.Models;

namespace GOG.Activities.List
{
    public class ListUpdatedActivity : Activity
    {
        IDataController<long> updatedDataController;
        IDataController<AccountProduct> accountProductsDataController;

        public ListUpdatedActivity(
            IDataController<long> updatedDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IStatusController statusController) :
            base(statusController)
        {
            this.updatedDataController = updatedDataController;
            this.accountProductsDataController = accountProductsDataController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var listUpdatedStatus = statusController.Create(status, "List updated account products");
            var current = 0;
            var updatedAccountProducts = new Dictionary<string, string>();

            foreach (var updatedId in updatedDataController.EnumerateIds())
            {
                var updatedIdString = updatedId.ToString();
                statusController.UpdateProgress(
                    listUpdatedStatus,
                    ++current,
                    updatedDataController.Count(),
                    updatedIdString);

                updatedAccountProducts.Add(
                    updatedIdString,
                    "(Account product not found)");

                var accountProduct = await accountProductsDataController.GetByIdAsync(updatedId);
                if (accountProduct == null)
                {
                    statusController.Warn(
                        listUpdatedStatus,
                        "Account product {updatedId} doesn't exist, but is marked as updated");
                    continue;
                }

                updatedAccountProducts[updatedIdString] = accountProduct.Title;

                // TODO: add table formatting controller
                // TODO: figure out how to add and post summaries for the activity like this one using consolePresentationController
            }

            statusController.Complete(listUpdatedStatus);
        }
    }
}
