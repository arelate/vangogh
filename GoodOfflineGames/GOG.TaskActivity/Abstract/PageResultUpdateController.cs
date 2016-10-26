using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.ProductTypes;
using Interfaces.Storage;

using Models.Uris;
using Models.ProductCore;

using GOG.Models;

using GOG.Interfaces.PageResults;
using GOG.Interfaces.Extraction;

namespace GOG.TaskActivities.Abstract
{
    public abstract class PageResultUpdateController<PageType, Type> : TaskActivityController 
        where PageType: PageResult 
        where Type : ProductCore
    {
        private IRequestPageController requestPageController;
        private ISerializationController<string> serializationController;
        private IProductTypeStorageController productStorageController;

        internal ProductTypes productType;
        internal IPageResultsController<PageType> pageResultsController;
        internal IPageResultsExtractionController<PageType, Type> pageResultsExtractingController;

        private string filenameTemplate = "{0}s.js";
        internal string filename;

        public PageResultUpdateController(
            IRequestPageController requestPageController,
            ISerializationController<string> serializationController,
            IProductTypeStorageController productStorageController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.requestPageController = requestPageController;
            this.serializationController = serializationController;
            this.productStorageController = productStorageController;

            filename = string.Format(filenameTemplate, productType.ToString().ToLower());
        }

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask("Update products from " + Uris.Paths.GetUpdateUri(productType));
            var productsPageResults = await pageResultsController.GetPageResults();

            taskReportingController.StartTask("Extract product data");
            var products = pageResultsExtractingController.Extract(productsPageResults);
            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Save products to disk");
            await productStorageController.Push(productType, products);
            taskReportingController.CompleteTask();

            taskReportingController.CompleteTask();
        }
    }
}
