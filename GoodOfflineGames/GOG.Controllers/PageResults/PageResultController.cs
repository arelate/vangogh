using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.Reporting;

using GOG.Interfaces.PageResults;

using GOG.Models;

namespace GOG.Controllers.PageResults
{
    public class PageResultsController<T>: IPageResultsController<T> where T : PageResult
    {
        private IRequestPageController requestPageController;
        private string uri;
        private IDictionary<string, string> parameters;
        private ISerializationController<string> serializationController;
        private IReportProgressDelegate reportProgressDelegate;

        public PageResultsController(
            IRequestPageController requestPageController,
            ISerializationController<string> serializationController,
            string uri,
            IDictionary<string, string> parameters,
            IReportProgressDelegate reportProgressDelegate = null)
        {
            this.requestPageController = requestPageController;
            this.serializationController = serializationController;
            this.uri = uri;
            this.parameters = parameters;
            this.reportProgressDelegate = reportProgressDelegate;
        }

        public async Task<IList<T>> GetPageResults()
        {
            var pageResults = new List<T>();
            var currentPage = 1;
            T pageResult = null;

            do
            {
                var response = await requestPageController.RequestPage(uri, parameters, currentPage);
                pageResult = serializationController.Deserialize<T>(response);

                if (reportProgressDelegate != null &&
                    pageResult != null)
                    reportProgressDelegate.ReportProgress(currentPage, pageResult.TotalPages);

                pageResults.Add(pageResult);

            } while (
                pageResult != null &&
                ++currentPage <= pageResult.TotalPages);

            return pageResults;
        }
    }
}
