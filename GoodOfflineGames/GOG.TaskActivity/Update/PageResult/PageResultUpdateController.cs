using System.Threading.Tasks;
using System.Linq;

using Interfaces.Reporting;
using Interfaces.RequestPage;
using Interfaces.ProductTypes;
using Interfaces.Data;

using Models.Uris;
using Models.ProductCore;

using GOG.Interfaces.PageResults;
using GOG.Interfaces.Extraction;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Update.PageResult
{
    public class PageResultUpdateController<PageType, Type> : TaskActivityController
        where PageType : Models.PageResult
        where Type : ProductCore
    {
        private ProductTypes productType;

        private IPageResultsController<PageType> pageResultsController;
        private IPageResultsExtractionController<PageType, Type> pageResultsExtractingController;

        private IRequestPageController requestPageController;
        private IDataController<Type> dataController;

        public PageResultUpdateController(
            ProductTypes productType,
            IPageResultsController<PageType> pageResultsController,
            IPageResultsExtractionController<PageType, Type> pageResultsExtractingController,
            IRequestPageController requestPageController,
            IDataController<Type> dataController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.productType = productType;

            this.pageResultsController = pageResultsController;
            this.pageResultsExtractingController = pageResultsExtractingController;

            this.requestPageController = requestPageController;
            this.dataController = dataController;
        }

        public override async Task ProcessTaskAsync()
        {
            taskReportingController.StartTask("Update products from " + Uris.Paths.GetUpdateUri(productType));
            var productsPageResults = await pageResultsController.GetPageResults();

            taskReportingController.StartTask("Extract product data");
            var products = pageResultsExtractingController.Extract(productsPageResults);
            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Update existing products");
            await dataController.UpdateAsync(products.ToArray());
            taskReportingController.CompleteTask();

            taskReportingController.CompleteTask();
        }
    }
}
