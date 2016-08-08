using System.Collections.Generic;

using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.Reporting;

using GOG.Models;

namespace GOG.Controllers.PageResults
{
    public class ProductsPageResultController : PageResultsController<ProductsPageResult>
    {
        public ProductsPageResultController(
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
