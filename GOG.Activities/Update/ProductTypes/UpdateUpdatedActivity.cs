using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Confirm;

using Interfaces.Controllers.Data;
using Interfaces.Controllers.Logs;

using Interfaces.Activity;

using Attributes;

using GOG.Models;

namespace GOG.Activities.Update.ProductTypes
{
    public class UpdateUpdatedActivity: IActivity
    {
        readonly IDataController<AccountProduct> accountProductDataController;
        readonly IConfirmDelegate<AccountProduct> confirmAccountProductUpdatedDelegate;

        readonly IDataController<long> updatedDataController;
        readonly IActionLogController actionLogController;

        [Dependencies(
            "GOG.Controllers.Data.ProductTypes.AccountProductsDataController,GOG.Controllers",
            "GOG.Delegates.Confirm.ProductTypes.ConfirmAccountProductUpdatedDelegate,GOG.Delegates",
            "Controllers.Data.ProductTypes.UpdatedDataController,Controllers",
            "Controllers.Logs.ActionLogController,Controllers")]
        public UpdateUpdatedActivity(
            IDataController<AccountProduct> accountProductDataController,
            IConfirmDelegate<AccountProduct> confirmAccountProductUpdatedDelegate,
            IDataController<long> updatedDataController,
            IActionLogController actionLogController)
        {
            this.accountProductDataController = accountProductDataController;
            this.confirmAccountProductUpdatedDelegate = confirmAccountProductUpdatedDelegate;

            this.updatedDataController = updatedDataController;
            this.actionLogController = actionLogController;
        }

        public async Task ProcessActivityAsync()
        {
            // This activity will centralize processing and marking updated account products.
            // Currently the process is the following:
            // 1) Itemize account products that were created after last updateData-accountProducts
            // Please note, that every updateData-accountProducts that results in new products will 
            // overwrite this timestamp, so it's expected that updateData-updated is run between the 
            // sessions that produce new account products.
            // 2) Itemize all account products and confirm is isNew or Updates passes the condition
            // ...
            // In the future additional heuristics can be employed - such as using products, not just 
            // account products and other. Currently they are considered as YAGNI

            actionLogController.StartAction("Process updated account products");

            actionLogController.StartAction("Add account products created since last data update");

            var accountProductsNewOrUpdated = new List<long>();

            //var lastUpdatedAccountProductsData = await activityContextCreatedIndexController.GetCreatedAsync(
            //    activityContextController.ToString((A.UpdateData, Context.AccountProducts)), addNewlyCreatedStatus);

            //var newlyCreatedAccountProducts = await accountProductDataController.ItemizeAsync(lastUpdatedAccountProductsData, addNewlyCreatedStatus);

            //accountProductsNewOrUpdated.AddRange(newlyCreatedAccountProducts);

            actionLogController.CompleteAction();

            actionLogController.StartAction("Add updated account products");

            await foreach (var accountProduct in accountProductDataController.ItemizeAllAsync())
            {
                actionLogController.IncrementActionProgress();

                if (confirmAccountProductUpdatedDelegate.Confirm(accountProduct))
                    accountProductsNewOrUpdated.Add(accountProduct.Id);
            }

            foreach (var accountProduct in accountProductsNewOrUpdated)
                await updatedDataController.UpdateAsync(accountProduct);

            actionLogController.CompleteAction();

            await updatedDataController.CommitAsync();

            actionLogController.CompleteAction();

        }
    }
}
