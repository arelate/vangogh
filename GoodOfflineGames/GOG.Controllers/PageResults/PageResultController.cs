using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.TaskStatus;
using Interfaces.ProductTypes;
using Interfaces.Hash;

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
        private IHashTrackingController hashTrackingController;
        private ISerializationController<string> serializationController;
        private ITaskStatusController taskStatusController;

        private string requestUri;
        private IDictionary<string, string> requestParameters;

        public PageResultsController(
            ProductTypes productType,
            IRequestPageController requestPageController,
            IHashTrackingController hashTrackingController,
            ISerializationController<string> serializationController,
            ITaskStatusController taskStatusController)
        {
            this.productType = productType;

            this.requestPageController = requestPageController;
            this.hashTrackingController = hashTrackingController;
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

                taskStatusController.UpdateProgress(
                    getPagesTask,
                    currentPage,
                    totalPages,
                    requestUri,
                    PageUnits.Pages);

                var requestHash = hashTrackingController.GetHash(requestUri + currentPage);
                var responseHash = response.GetHashCode();

                pageResult = serializationController.Deserialize<T>(response);

                if (pageResult == null) continue;

                totalPages = pageResult.TotalPages;

                if (responseHash == requestHash) continue;

                var setHashTask = taskStatusController.Create(getPagesTask, "Set response hash");
                await hashTrackingController.SetHashAsync(requestUri + currentPage, responseHash);
                taskStatusController.Complete(setHashTask);

                pageResults.Add(pageResult);

            } while (++currentPage <= totalPages);

            taskStatusController.Complete(getPagesTask);

            return pageResults;
        }
    }
}
