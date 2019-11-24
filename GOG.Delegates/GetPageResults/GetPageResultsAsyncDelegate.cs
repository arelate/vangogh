﻿using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;

using Interfaces.Controllers.Hashes;

using Interfaces.Controllers.Serialization;
using Interfaces.Status;
using Interfaces.Models.Entities;

using Models.Units;

using GOG.Interfaces.Delegates.GetPageResults;
using GOG.Interfaces.Delegates.RequestPage;

using GOG.Models;

namespace GOG.Delegates.GetPageResults
{
    public abstract class GetPageResultsAsyncDelegate<T> : IGetPageResultsAsyncDelegate<T> where T : PageResult
    {
        readonly IGetValueDelegate<string> getPageResultsUpdateUriDelegate;
        readonly IGetValueDelegate<Dictionary<string, string>> getPageResultsUpdateQueryParametersDelegate;
        readonly IRequestPageAsyncDelegate requestPageAsyncDelegate;
        readonly IConvertAsyncDelegate<string, Task<string>> convertStringToHashDelegate;
        readonly IHashesController hashesController;
        readonly ISerializationController<string> serializationController;
        readonly IStatusController statusController;

        public GetPageResultsAsyncDelegate(
            IGetValueDelegate<string> getPageResultsUpdateUriDelegate,
            IGetValueDelegate<Dictionary<string, string>> getPageResultsUpdateQueryParametersDelegate,
            IRequestPageAsyncDelegate requestPageAsyncDelegate,
            IConvertAsyncDelegate<string, Task<string>> convertStringToHashDelegate,
            IHashesController storedHashController,
            ISerializationController<string> serializationController,
            IStatusController statusController)
        {
            this.getPageResultsUpdateUriDelegate = getPageResultsUpdateUriDelegate;
            this.getPageResultsUpdateQueryParametersDelegate = getPageResultsUpdateQueryParametersDelegate;

            this.requestPageAsyncDelegate = requestPageAsyncDelegate;
            this.convertStringToHashDelegate = convertStringToHashDelegate;
            this.hashesController = storedHashController;
            this.serializationController = serializationController;

            this.statusController = statusController;
        }

        public async Task<IList<T>> GetPageResultsAsync(IStatus status)
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

            var requestUri = getPageResultsUpdateUriDelegate.GetValue();
            var requestParameters = getPageResultsUpdateQueryParametersDelegate.GetValue();

            var getPagesTask = await statusController.CreateAsync(status, $"Request pages data");

            do
            {
                var response = await requestPageAsyncDelegate.RequestPageAsync(
                    requestUri, 
                    requestParameters, 
                    currentPage,
                    getPagesTask);

                await statusController.UpdateProgressAsync(
                    getPagesTask,
                    currentPage,
                    totalPages,
                    requestUri,
                    PageUnits.Pages);

                var requestHash = await hashesController.ConvertAsync(requestUri + currentPage, getPagesTask);
                var responseHash = await convertStringToHashDelegate.ConvertAsync(response, getPagesTask);

                pageResult = serializationController.Deserialize<T>(response);

                if (pageResult == null) continue;

                totalPages = pageResult.TotalPages;

                if (responseHash == requestHash) continue;

                var setHashTask = await statusController.CreateAsync(getPagesTask, "Set hash", false);
                await hashesController.SetHashAsync(requestUri + currentPage, responseHash, setHashTask);
                await statusController.CompleteAsync(setHashTask, false);

                pageResults.Add(pageResult);

            } while (++currentPage <= totalPages);

            await statusController.CompleteAsync(getPagesTask);

            return pageResults;
        }
    }
}
