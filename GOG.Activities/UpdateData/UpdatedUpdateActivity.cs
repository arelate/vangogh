using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Delegates.Confirm;

using Interfaces.Controllers.Index;
using Interfaces.Controllers.Data;

using Interfaces.Status;

using A = Interfaces.ActivityDefinitions.Activity;
using Interfaces.ContextDefinitions;
using Interfaces.ActivityContext;

using GOG.Models;

namespace GOG.Activities.UpdateData
{
    public class UpdatedUpdateActivity: Activity
    {
        private IActivityContextController activityContextController;
        private IIndexController<string> activityContextCreatedIndexController;

        private IDataController<AccountProduct> accountProductDataController;
        private IConfirmDelegate<AccountProduct> confirmAccountProductIsNewDelegate;
        private IConfirmDelegate<AccountProduct> confirmAccountProductHasUpdatesDelegate;

        public UpdatedUpdateActivity(
            IActivityContextController activityContextController,
            IIndexController<string> activityContextCreatedIndexController,
            IDataController<AccountProduct> accountProductDataController,
            IConfirmDelegate<AccountProduct> confirmAccountProductIsNewDelegate,
            IConfirmDelegate<AccountProduct> confirmAccountProductHasUpdatesDelegate,
            IStatusController statusController): base(statusController)
        {
            this.activityContextController = activityContextController;
            this.activityContextCreatedIndexController = activityContextCreatedIndexController;

            this.accountProductDataController = accountProductDataController;
            this.confirmAccountProductIsNewDelegate = confirmAccountProductIsNewDelegate;
            this.confirmAccountProductHasUpdatesDelegate = confirmAccountProductHasUpdatesDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
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

            var lastUpdatedAccountProductsData = await activityContextCreatedIndexController.GetCreatedAsync(
                activityContextController.ToString((A.UpdateData, Context.AccountProducts)), status);

            var newAccountProducts = await accountProductDataController.ItemizeAsync(lastUpdatedAccountProductsData, status);
        }
    }
}
