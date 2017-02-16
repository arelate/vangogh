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

        private string requestUri;
        private IDictionary<string, string> requestParameters;

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

            requestUri = Uris.Paths.GetUpdateUri(productType);
            requestParameters = QueryParameters.GetQueryParameters(productType);
        }

        public async Task<IList<T>> GetPageResults(ITaskStatus taskStatus)
        {
            var pageResults = new List<T>();
            var currentPage = 1;
            var totalPages = 1;
            T pageResult = null;



            var getPagesTask = taskStatusController.Create(
                taskStatus,
                string.Format(
                    "Get all pages for {0}",
                    productType.ToString()));

            do
            {
                var response = await requestPageController.RequestPage(
                    requestUri, 
                    requestParameters, 
                    currentPage);

                pageResult = serializationController.Deserialize<T>(response);

                taskStatusController.UpdateProgress(
                    getPagesTask,
                    currentPage,
                    totalPages,
                    requestUri,
                    PageUnits.Pages);

                if (pageResult == null) continue;

                totalPages = pageResult.TotalPages;

                pageResults.Add(pageResult);

            } while (++currentPage <= totalPages);

            taskStatusController.Complete(getPagesTask);

            return pageResults;
        }
    }
}
