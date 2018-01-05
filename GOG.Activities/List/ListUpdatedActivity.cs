using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Status;
using Interfaces.Console;

using GOG.Models;

namespace GOG.Activities.List
{
    public class ListUpdatedActivity : Activity
    {
        IDataController<long> updatedDataController;
        IDataController<AccountProduct> accountProductsDataController;

        IConsoleController consoleController; // temp. while we wait clarity on summary results

        public ListUpdatedActivity(
            IDataController<long> updatedDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IStatusController statusController,
            IConsoleController consoleController) :
            base(statusController)
        {
            this.updatedDataController = updatedDataController;
            this.accountProductsDataController = accountProductsDataController;

            this.consoleController = consoleController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var listUpdatedStatus = await statusController.CreateAsync(status, "List updated account products");
            var current = 0;
            var count = await updatedDataController.CountAsync(listUpdatedStatus);
            var updatedAccountProducts = new Dictionary<long, string>();

            foreach (var updatedId in await updatedDataController.EnumerateIdsAsync(listUpdatedStatus))
            {
                await statusController.UpdateProgressAsync(
                    listUpdatedStatus,
                    ++current,
                    count,
                    updatedId.ToString());

                updatedAccountProducts.Add(
                    updatedId,
                    "(Account product not found)");

                var accountProduct = await accountProductsDataController.GetByIdAsync(updatedId, listUpdatedStatus);
                if (accountProduct == null)
                {
                    await statusController.WarnAsync(
                        listUpdatedStatus,
                        "Account product {updatedId} doesn't exist, but is marked as updated");
                    continue;
                }

                updatedAccountProducts[updatedId] = accountProduct.Title;


                // TODO: add updated viewModel formatting controller
                // TODO: figure out how to add and post summaries for the activity like this one using consolePresentationController
            }

            await statusController.CompleteAsync(listUpdatedStatus);
        }
    }
}
