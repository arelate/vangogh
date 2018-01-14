using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        public UpdatedUpdateActivity(
            IActivityContextController activityContextController,
            IIndexController<string> activityContextCreatedIndexController,
            IDataController<AccountProduct> accountProductDataController,
            IStatusController statusController): base(statusController)
        {
            this.activityContextController = activityContextController;
            this.activityContextCreatedIndexController = activityContextCreatedIndexController;

            this.accountProductDataController = accountProductDataController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var lastUpdatedAccountProductsData = await activityContextCreatedIndexController.GetCreatedAsync(
                activityContextController.ToString((A.UpdateData, Context.AccountProducts)), status);

            var newAccountProducts = await accountProductDataController.ItemizeAsync(lastUpdatedAccountProductsData, status);
        }
    }
}
