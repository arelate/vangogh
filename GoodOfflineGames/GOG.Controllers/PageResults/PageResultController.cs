using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.TaskStatus;
using Interfaces.ProductTypes;
using Interfaces.ForEachAsync;

using Models.Uris;
using Models.QueryParameters;

using GOG.Interfaces.PageResults;

using GOG.Models;

namespace GOG.Controllers.PageResults
{
    public class PageResultsController<T> : IPageResultsController<T> where T : PageResult
    {
        private ProductTypes productType;
        private IRequestPageController requestPageController;
        private ISerializationController<string> serializationController;
        private IForEachAsyncDelegate forEachAsyncDelegate;
        private ITaskStatusController taskStatusController;

        private string requestUri;
        private Dictionary<string, string> requestParameters;

        public PageResultsController(
            ProductTypes productType,
            IRequestPageController requestPageController,
            ISerializationController<string> serializationController,
            IForEachAsyncDelegate forEachAsyncDelegate,
            ITaskStatusController taskStatusController)
        {
            this.productType = productType;

            requestUri = Uris.Paths.GetUpdateUri(productType);
            requestParameters = QueryParameters.GetQueryParameters(productType);

            this.requestPageController = requestPageController;
            this.serializationController = serializationController;
            this.forEachAsyncDelegate = forEachAsyncDelegate;

            this.taskStatusController = taskStatusController;
        }

        public async Task<IList<T>> GetPageResults(ITaskStatus taskStatus)
        {
            var pageResults = new List<T>();
            var currentPage = 1;

            var requestAllPagesTask = taskStatusController.Create(
                taskStatus,
                string.Format(
                    "Request all pages for {0}",
                    productType.ToString()));

            // request first page to determine total pages count

            var requestFirstPageTask = taskStatusController.Create(
                requestAllPagesTask,
                "Request first page");

            var firstResponse = await requestPageController.RequestPage(
                requestUri,
                requestParameters,
                currentPage);

            var firstPageResult = serializationController.Deserialize<T>(firstResponse);

            if (firstPageResult == null)
            {
                taskStatusController.Fail(taskStatus, "Requested page returned null");
                return null;
            }

            pageResults.Add(firstPageResult);
            var totalPages = firstPageResult.TotalPages;

            // if there is only one results page - no need to do anything else
            if (currentPage == totalPages) return pageResults;

            taskStatusController.Complete(requestFirstPageTask);

            // get responses for all remaining pages in parallel

            var requestRemainingPagesTask = taskStatusController.Create(
                requestAllPagesTask,
                "Request remaining page(s)");

            var responses = new List<string>();
            var pages = Enumerable.Range(2, totalPages - 1);

            await forEachAsyncDelegate.ForEachAsync(pages, async (page) =>
            {
                var response = await requestPageController.RequestPage(
                requestUri,
                requestParameters,
                page);

                taskStatusController.UpdateProgress(
                    requestRemainingPagesTask, 
                    page, 
                    totalPages, 
                    requestUri);

                responses.Add(response);
            });

            taskStatusController.Complete(requestRemainingPagesTask);

            // finally, deserialize responses and add to results

            var deserializeResonsesTask = taskStatusController.Create(requestAllPagesTask, "Deserialize response(s) and add to page results");

            foreach (var response in responses)
            {
                var pageResult = serializationController.Deserialize<T>(response);
                if (pageResult == null) continue;

                pageResults.Add(pageResult);
            }

            taskStatusController.Complete(deserializeResonsesTask);

            taskStatusController.Complete(requestAllPagesTask);

            return pageResults;
        }
    }
}
