using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Interfaces.Collection;
using Interfaces.Data;
using Interfaces.TaskStatus;
using Interfaces.DataRefinement;

using GOG.Models;

namespace GOG.Controllers.DataRefinement
{
    public class NewUpdatedDataRefinementController : IDataRefinementController<AccountProduct>
    {
        private ICollectionController collectionController;
        private IDataController<long> updatedDataController;
        private IDataController<long> lastKnownValidDataController;
        private ITaskStatusController taskStatusController;

        public NewUpdatedDataRefinementController(
            ICollectionController collectionController,
            IDataController<long> updatedDataController,
            IDataController<long> lastKnownValidDataController,
            ITaskStatusController taskStatusController)
        {
            this.collectionController = collectionController;
            this.updatedDataController = updatedDataController;
            this.lastKnownValidDataController = lastKnownValidDataController;
            this.taskStatusController = taskStatusController;
        }

        public async Task RefineData(IEnumerable<AccountProduct> accountProducts, ITaskStatus taskStatus)
        {
            // GOG.com quirk
            // There are few ways to get new and updated products:
            // 1) Scan AccountProducts and check IsNew flag and number of Updates
            // 2) Do a special request with relevant queryParameters - 
            //    and this is what GOG.com actually does when you select "Updated" as a filter.
            // In the earlier app version we used 2), then a separate form of 1), basically
            // going through all AccountProducts and checking the flags. However with the move to 
            // DataControllers where actual AccountProducts are not stored in memory and have to be loaded
            // from storage - this was very slow method. Now instead of doing this every time we use
            // collection processing as the last step of AccountProducts update. There are several benefits
            // to this approach - since AccountProducts is filtering our results that have same hash
            // as previous version we normally don't process a lot of AccountProducts here.

            var extractNewUpdatedTask = taskStatusController.Create(taskStatus, "Extract new and updated products");

            var newUpdatedAccountProducts = collectionController.Reduce(accountProducts,
                ap =>
                {
                    return ap.IsNew || ap.Updates > 0;
                });

            var updatedIds = newUpdatedAccountProducts.Select(ap => ap.Id).ToArray();

            await updatedDataController.UpdateAsync(extractNewUpdatedTask, updatedIds);

            await lastKnownValidDataController.RemoveAsync(extractNewUpdatedTask, updatedIds);

            taskStatusController.Complete(extractNewUpdatedTask);
        }
    }
}
