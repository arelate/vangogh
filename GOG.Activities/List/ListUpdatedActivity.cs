using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Index;

using Interfaces.Status;

using Interfaces.Controllers.Console;

using GOG.Models;

namespace GOG.Activities.List
{
    public class ListUpdatedActivity : Activity
    {
        IIndexController<long> updatedDataController;
        IDataController<AccountProduct> accountProductsDataController;

        public ListUpdatedActivity(
            IIndexController<long> updatedDataController,
            IDataController<AccountProduct> accountProductsDataController,
            IStatusController statusController) :
            base(statusController)
        {
            this.updatedDataController = updatedDataController;
            this.accountProductsDataController = accountProductsDataController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var listUpdatedStatus = await statusController.CreateAsync(status, "List updated account products");
            var current = 0;
            var count = await updatedDataController.CountAsync(listUpdatedStatus);
            var updatedAccountProducts = new Dictionary<long, string>();

            foreach (var updatedId in await updatedDataController.ItemizeAllAsync(listUpdatedStatus))
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
