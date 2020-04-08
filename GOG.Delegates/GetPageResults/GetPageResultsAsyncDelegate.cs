using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetValue;
using Interfaces.Delegates.Activities;
using Models.Units;
using GOG.Interfaces.Delegates.GetPageResults;
using GOG.Interfaces.Delegates.RequestPage;
using GOG.Models;

namespace GOG.Delegates.GetPageResults
{
    public abstract class GetPageResultsAsyncDelegate<T> : IGetPageResultsAsyncDelegate<T> where T : PageResult
    {
        private readonly IGetValueDelegate<string> getPageResultsUpdateUriDelegate;
        private readonly IGetValueDelegate<Dictionary<string, string>> getPageResultsUpdateQueryParametersDelegate;
        private readonly IRequestPageAsyncDelegate requestPageAsyncDelegate;
        private readonly IConvertDelegate<string, T> convertJSONToTypeDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;

        public GetPageResultsAsyncDelegate(
            IGetValueDelegate<string> getPageResultsUpdateUriDelegate,
            IGetValueDelegate<Dictionary<string, string>> getPageResultsUpdateQueryParametersDelegate,
            IRequestPageAsyncDelegate requestPageAsyncDelegate,
            IConvertDelegate<string, T> convertJSONToTypeDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.getPageResultsUpdateUriDelegate = getPageResultsUpdateUriDelegate;
            this.getPageResultsUpdateQueryParametersDelegate = getPageResultsUpdateQueryParametersDelegate;

            this.requestPageAsyncDelegate = requestPageAsyncDelegate;
            this.convertJSONToTypeDelegate = convertJSONToTypeDelegate;

            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task<IList<T>> GetPageResultsAsync()
        {
            // GOG.com quirk
            // Products, AccountProducts use server-side paginated results, similar to Wikipedia.
            // This class encapsulates requesting sequence of pages up to the total number of pages.
            // Please note that ealier app versions also used heuristic optimization when
            // some page was unchanged - stop requesting next pages. This leads to stale data as GOG.com 
            // changes information all the time updating Products and AccountProducts. It's especially
            // important for AccountProducts as GOG.com can set Updates information on any AccountProduct. 
            // Updated is used for driving updated.json - that in turn is used for all subsequent operations 
            // as an optimization - we don't process all products all the time, only updated

            var pageResults = new List<T>();
            var currentPage = 1;
            var totalPages = 1;
            T pageResult = null;

            var requestUri = getPageResultsUpdateUriDelegate.GetValue();
            var requestParameters = getPageResultsUpdateQueryParametersDelegate.GetValue();

            startDelegate.Start($"Request pages data");

            do
            {
                var response = await requestPageAsyncDelegate.RequestPageAsync(
                    requestUri,
                    requestParameters,
                    currentPage);

                setProgressDelegate.SetProgress();

                pageResult = convertJSONToTypeDelegate.Convert(response);

                if (pageResult == null) continue;

                totalPages = pageResult.TotalPages;

                pageResults.Add(pageResult);
            } while (++currentPage <= totalPages);

            completeDelegate.Complete();

            return pageResults;
        }
    }
}