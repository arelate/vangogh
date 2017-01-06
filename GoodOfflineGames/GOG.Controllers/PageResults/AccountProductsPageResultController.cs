using Interfaces.RequestPage;
using Interfaces.Serialization;
using Interfaces.TaskStatus;
using Interfaces.ProductTypes;

using GOG.Models;

namespace GOG.Controllers.PageResults
{
    public class AccountProductsPageResultController : PageResultsController<AccountProductsPageResult>
    {
        public AccountProductsPageResultController(
            ProductTypes productType,
            IRequestPageController requestPageController,
            ISerializationController<string> serializationController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController) : 
            base(
                productType,
                requestPageController, 
                serializationController, 
                taskStatus,
                taskStatusController)
        {
            // ...
        }
    }
}
