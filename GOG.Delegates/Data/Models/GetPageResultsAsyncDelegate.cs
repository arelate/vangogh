using System.Collections.Generic;
using System.Threading.Tasks;
using GOG.Models;
using Interfaces.Delegates.Activities;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Models.QueryParameters;

namespace GOG.Delegates.Data.Models
{
    public abstract class GetPageResultsAsyncDelegate<T> : 
        IGetDataAsyncDelegate<IList<T>, string> 
        where T : PageResult
    {
        private readonly IGetValueDelegate<string, string> getPageResultsUpdateUriDelegate;
        private readonly IGetValueDelegate<Dictionary<string, string>, string> getPageResultsUpdateQueryParametersDelegate;
        private readonly IConvertDelegate<(string, IDictionary<string, string>), string>
            convertUriParametersToUriDelegate;

        private readonly IGetDataAsyncDelegate<string, string> getUriDataAsyncDelegate;
        private readonly IConvertDelegate<string, T> convertJSONToTypeDelegate;
        private readonly IStartDelegate startDelegate;
        private readonly ISetProgressDelegate setProgressDelegate;
        private readonly ICompleteDelegate completeDelegate;
        
        public GetPageResultsAsyncDelegate(
            IGetValueDelegate<string, string> getPageResultsUpdateUriDelegate,
            IGetValueDelegate<Dictionary<string, string>, string> getPageResultsUpdateQueryParametersDelegate,
            IConvertDelegate<(string, IDictionary<string, string>), string>
                convertUriParametersToUriDelegate,
            IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate,            
            IConvertDelegate<string, T> convertJSONToTypeDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate)
        {
            this.getPageResultsUpdateUriDelegate = getPageResultsUpdateUriDelegate;
            this.getPageResultsUpdateQueryParametersDelegate = getPageResultsUpdateQueryParametersDelegate;
            this.convertUriParametersToUriDelegate = convertUriParametersToUriDelegate;

            this.getUriDataAsyncDelegate = getUriDataAsyncDelegate;
            this.convertJSONToTypeDelegate = convertJSONToTypeDelegate;

            this.startDelegate = startDelegate;
            this.setProgressDelegate = setProgressDelegate;
            this.completeDelegate = completeDelegate;
        }

        public async Task<IList<T>> GetDataAsync(string uri)
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

            var requestUri = getPageResultsUpdateUriDelegate.GetValue(
                string.Empty);
            var requestParameters = getPageResultsUpdateQueryParametersDelegate.GetValue(
                string.Empty);

            startDelegate.Start($"Request pages data");

            do
            {
                if (!requestParameters.ContainsKey(QueryParameters.Page))
                    requestParameters.Add(
                        QueryParameters.Page, 
                        currentPage.ToString());

                requestParameters[QueryParameters.Page] = currentPage.ToString();
                
                var uriParameters = convertUriParametersToUriDelegate.Convert((requestUri, requestParameters));
                
                var response = await getUriDataAsyncDelegate.GetDataAsync(uriParameters);

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