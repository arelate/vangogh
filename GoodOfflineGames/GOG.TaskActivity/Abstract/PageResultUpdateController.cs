using System.Threading.Tasks;
using System.Linq;

using Interfaces.Reporting;
using Interfaces.RequestPage;
using Interfaces.ProductTypes;
using Interfaces.Data;

using Models.Uris;
using Models.ProductCore;

using GOG.Models;

using GOG.Interfaces.PageResults;
using GOG.Interfaces.Extraction;

namespace GOG.TaskActivities.Abstract
{
    public abstract class PageResultUpdateController<PageType, Type> : TaskActivityController
        where PageType : PageResult
        where Type : ProductCore
    {
        private IRequestPageController requestPageController;
        private IDataController<Type> dataController;

        internal ProductTypes productType;
        internal IPageResultsController<PageType> pageResultsController;
        internal IPageResultsExtractionController<PageType, Type> pageResultsExtractingController;

        public PageResultUpdateController(
            IRequestPageController requestPageController,
            IDataController<Type> dataController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.requestPageController = requestPageController;
            this.dataController = dataController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask("Update products from " + Uris.Paths.GetUpdateUri(productType));
            var productsPageResults = await pageResultsController.GetPageResults();

            taskReportingController.StartTask("Extract product data");
            var products = pageResultsExtractingController.Extract(productsPageResults);
            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Update existing products");
            await dataController.Update(products.ToArray());
            taskReportingController.CompleteTask();

            taskReportingController.CompleteTask();
        }
    }
}
