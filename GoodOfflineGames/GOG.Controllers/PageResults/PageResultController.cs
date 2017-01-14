using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.TaskStatus;
using Interfaces.ProductTypes;

using Models.Uris;
using Models.QueryParameters;
using Models.Units;

using GOG.Interfaces.PageResults;

using GOG.Models;

namespace GOG.Controllers.PageResults
{
    public class PageResultsController<T> : IPageResultsController<T> where T : PageResult
    {
        private ProductTypes productType;
        private IRequestPageController requestPageController;
        private ISerializationController<string> serializationController;
        private ITaskStatusController taskStatusController;

        public PageResultsController(
            ProductTypes productType,
            IRequestPageController requestPageController,
            ISerializationController<string> serializationController,
            ITaskStatusController taskStatusController)
        {
            this.productType = productType;

            this.requestPageController = requestPageController;
            this.serializationController = serializationController;

            this.taskStatusController = taskStatusController;
        }

        public async Task<IList<T>> GetPageResults(ITaskStatus taskStatus)
        {
            var pageResults = new List<T>();
            var currentPage = 1;
            T pageResult = null;

            var uri = Uris.Paths.GetUpdateUri(productType);
            var parameters = QueryParameters.GetQueryParameters(productType);

            var getPagesTask = taskStatusController.Create(
                taskStatus, 
                string.Format(
                    "Get all pages for {0}",
                    productType.ToString()));

            do
            {
                var response = await requestPageController.RequestPage(uri, parameters, currentPage);
                pageResult = serializationController.Deserialize<T>(response);

                taskStatusController.UpdateProgress(
                    getPagesTask, 
                    currentPage, 
                    pageResult.TotalPages, 
                    uri, 
                    PageUnits.Pages);

                pageResults.Add(pageResult);

            } while (
                pageResult != null &&
                ++currentPage <= pageResult.TotalPages);

            taskStatusController.Complete(getPagesTask);

            return pageResults;
        }
    }
}
