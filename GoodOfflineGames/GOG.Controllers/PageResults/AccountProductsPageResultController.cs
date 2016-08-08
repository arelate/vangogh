using System.Collections.Generic;

using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.Reporting;

using GOG.Models;

namespace GOG.Controllers.PageResults
{
    public class AccountProductsPageResultController : PageResultsController<AccountProductsPageResult>
    {
        public AccountProductsPageResultController(
            IRequestPageController requestPageController,
            ISerializationController<string> serializationController,
            string uri,
            IDictionary<string, string> parameters,
            IReportProgressDelegate reportProgressDelegate) : 
            base(
                requestPageController, 
                serializationController, 
                uri, 
                parameters,
                reportProgressDelegate)
        {
            // ...
        }
    }
}
