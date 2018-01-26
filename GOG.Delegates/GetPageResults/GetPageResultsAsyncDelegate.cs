using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.GetQueryParameters;
using Interfaces.Delegates.Hash;

using Interfaces.Controllers.Hash;

using Interfaces.Serialization;
using Interfaces.Status;
using Interfaces.ContextDefinitions;

using Models.Units;

using GOG.Interfaces.Delegates.GetPageResults;
using GOG.Interfaces.Delegates.GetUpdateUri;
using GOG.Interfaces.Delegates.RequestPage;

using GOG.Models;

namespace GOG.Delegates.GetPageResults
{
    public class GetPageResultsAsyncDelegate<T> : IGetPageResultsAsyncDelegate<T> where T : PageResult
    {
        private Context context;
        private IGetUpdateUriDelegate<Context> getUpdateUriDelegate;
        private IGetQueryParametersDelegate<Context> getQueryParametersDelegate;
        private IRequestPageAsyncDelegate requestPageAsyncDelegate;
        private IGetHashAsyncDelegate<string> getStringHashDelegate;
        private IStoredHashController storedHashController;
        private ISerializationController<string> serializationController;
        private IStatusController statusController;

        private string requestUri;
        private IDictionary<string, string> requestParameters;

        public GetPageResultsAsyncDelegate(
            Context context,
            IGetUpdateUriDelegate<Context> getUpdateUriDelegate,
            IGetQueryParametersDelegate<Context> getQueryParametersDelegate,
            IRequestPageAsyncDelegate requestPageAsyncDelegate,
            IGetHashAsyncDelegate<string> getStringHashDelegate,
            IStoredHashController storedHashController,
            ISerializationController<string> serializationController,
            IStatusController statusController)
        {
            this.context = context;
            this.getUpdateUriDelegate = getUpdateUriDelegate;
            this.getQueryParametersDelegate = getQueryParametersDelegate;

            this.requestPageAsyncDelegate = requestPageAsyncDelegate;
            this.getStringHashDelegate = getStringHashDelegate;
            this.storedHashController = storedHashController;
            this.serializationController = serializationController;

            this.statusController = statusController;

            requestUri = getUpdateUriDelegate.GetUpdateUri(context);
            requestParameters = getQueryParametersDelegate.GetQueryParameters(context);
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

            var getPagesTask = await statusController.CreateAsync(status, $"Request {context}");

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

                var requestHash = await storedHashController.GetHashAsync(requestUri + currentPage, getPagesTask);
                var responseHash = await getStringHashDelegate.GetHashAsync(response, getPagesTask);

                pageResult = serializationController.Deserialize<T>(response);

                if (pageResult == null) continue;

                totalPages = pageResult.TotalPages;

                if (responseHash == requestHash) continue;

                var setHashTask = await statusController.CreateAsync(getPagesTask, "Set hash", false);
                await storedHashController.SetHashAsync(requestUri + currentPage, responseHash, setHashTask);
                await statusController.CompleteAsync(setHashTask, false);

                pageResults.Add(pageResult);

            } while (++currentPage <= totalPages);

            await statusController.CompleteAsync(getPagesTask);

            return pageResults;
        }
    }
}
