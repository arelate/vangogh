﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
        private IConfirmDelegate<AccountProduct> confirmAccountProductUpdatedDelegate;

        private IIndexController<long> updatedIndexController;

        public UpdatedUpdateActivity(
            IActivityContextController activityContextController,
            IIndexController<string> activityContextCreatedIndexController,
            IDataController<AccountProduct> accountProductDataController,
            IConfirmDelegate<AccountProduct> confirmAccountProductUpdatedDelegate,
            IIndexController<long> updatedIndexController,
            IStatusController statusController): base(statusController)
        {
            this.activityContextController = activityContextController;
            this.activityContextCreatedIndexController = activityContextCreatedIndexController;

            this.accountProductDataController = accountProductDataController;
            this.confirmAccountProductUpdatedDelegate = confirmAccountProductUpdatedDelegate;

            this.updatedIndexController = updatedIndexController;
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

            var updateDataUpdatedStatus = await statusController.CreateAsync(status, "Process updated account products");

            var addNewlyCreatedStatus = await statusController.CreateAsync(updateDataUpdatedStatus, "Add account products created since last data update");

            var accountProductsNewOrUpdated = new List<long>();

            var lastUpdatedAccountProductsData = await activityContextCreatedIndexController.GetCreatedAsync(
                activityContextController.ToString((A.UpdateData, Context.AccountProducts)), addNewlyCreatedStatus);

            var newlyCreatedAccountProducts = await accountProductDataController.ItemizeAsync(lastUpdatedAccountProductsData, addNewlyCreatedStatus);

            accountProductsNewOrUpdated.AddRange(newlyCreatedAccountProducts);

            await statusController.CompleteAsync(addNewlyCreatedStatus);

            var addUpdatedAccountProductsStatus = await statusController.CreateAsync(updateDataUpdatedStatus, "Add updated account products");

            var current = 0;

            foreach (var id in await accountProductDataController.ItemizeAllAsync(addUpdatedAccountProductsStatus))
            {
                await statusController.UpdateProgressAsync(
                    addUpdatedAccountProductsStatus,
                    ++current,
                    await accountProductDataController.CountAsync(addUpdatedAccountProductsStatus),
                    id.ToString());

                var accountProduct = await accountProductDataController.GetByIdAsync(id, status);

                if (confirmAccountProductUpdatedDelegate.Confirm(accountProduct))
                    accountProductsNewOrUpdated.Add(id);
            }

            await updatedIndexController.UpdateAsync(addUpdatedAccountProductsStatus, accountProductsNewOrUpdated.ToArray());

            await statusController.CompleteAsync(addUpdatedAccountProductsStatus);

            await statusController.CompleteAsync(updateDataUpdatedStatus);

        }
    }
}
