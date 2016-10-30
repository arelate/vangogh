using System.Threading.Tasks;

using Interfaces.Data;

using Interfaces.UpdateDependencies;

namespace GOG.TaskActivities.Update.Dependencies.GameProductData
{
    public class GameProductDataSkipUpdateController : ISkipUpdateController
    {
        private IDataController<Models.Product> productsDataController;

        public GameProductDataSkipUpdateController(IDataController<Models.Product> productsDataController)
        {
            this.productsDataController = productsDataController;
        }

        public async Task<bool> SkipUpdate(long id)
        {
            var product = await productsDataController.GetById(id);
            return product == null || product.IsComingSoon;
        }
    }
}
