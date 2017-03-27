﻿using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.TaskStatus;
using Interfaces.UpdateUri;
using Interfaces.Hash;
using Interfaces.QueryParameters;

using Models.Units;

using GOG.Interfaces.PageResults;

using GOG.Models;

namespace GOG.Controllers.PageResults
{
    public class PageResultsController<T> : IPageResultsController<T> where T : PageResult
    {
        private string productParameter;
        private IGetUpdateUriDelegate<string> getUpdateUriDelegate;
        private IGetQueryParametersDelegate<string> getQueryParametersDelegate;
        private IRequestPageController requestPageController;
        private IHashTrackingController hashTrackingController;
        private ISerializationController<string> serializationController;
        private ITaskStatusController taskStatusController;

        private string requestUri;
        private IDictionary<string, string> requestParameters;

        public PageResultsController(
            string productParameter,
            IGetUpdateUriDelegate<string> getUpdateUriDelegate,
            IGetQueryParametersDelegate<string> getQueryParametersDelegate,
            IRequestPageController requestPageController,
            IHashTrackingController hashTrackingController,
            ISerializationController<string> serializationController,
            ITaskStatusController taskStatusController)
        {
            this.productParameter = productParameter;
            this.getUpdateUriDelegate = getUpdateUriDelegate;
            this.getQueryParametersDelegate = getQueryParametersDelegate;

            this.requestPageController = requestPageController;
            this.hashTrackingController = hashTrackingController;
            this.serializationController = serializationController;

            this.taskStatusController = taskStatusController;

            requestUri = getUpdateUriDelegate.GetUpdateUri(productParameter);
            requestParameters = getQueryParametersDelegate.GetQueryParameters(productParameter);
        }

        public async Task<IList<T>> GetPageResults(ITaskStatus taskStatus)
        {
            // GOG.com quirk
            // Products, AccountProducts use server-side paginated results, similar to Wikipedia.
            // This class encapsulates requesting sequence of pages up to the total number of pages.
            // Additionally page requests are filtered using hashes, so if response has the same hash
            // we would not deserialize it again - no point passing around same data.
            // Please note that ealier app versions also used heuristic optimization when
            // some page was unchanged - stop requesting next pages. This leads to stale data as GOG.com 
            // changes information all the time updating Products and AccountProducts. It's especially
            // important for AccountProducts as GOG.com can set Updates information on any AccountProduct. 
            // Updated is used for driving updated.json - that in turn is used for all subsequent operations 
            // as an optimization - we don't process all products all the time, just updated

            var pageResults = new List<T>();
            var currentPage = 1;
            var totalPages = 1;
            T pageResult = null;

            var getPagesTask = taskStatusController.Create(taskStatus, $"Get all pages for {productParameter}");

            do
            {
                var response = await requestPageController.RequestPage(
                    requestUri, 
                    requestParameters, 
                    currentPage,
                    getPagesTask);

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
