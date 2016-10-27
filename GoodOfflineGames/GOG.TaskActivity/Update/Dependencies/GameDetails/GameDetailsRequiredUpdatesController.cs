using System.Linq;
using System.Threading.Tasks;

using Interfaces.ProductTypes;
using Interfaces.Storage;
using Interfaces.UpdateDependencies;

namespace GOG.TaskActivities.Update.Dependencies.GameDetails
{
    public class GameDetailsRequiredUpdatesController : IRequiredUpdatesController
    {
        //IProductTypeStorageController productStorageController;

        public GameDetailsRequiredUpdatesController(
            //IProductTypeStorageController productStorageController
            )
        {
            //this.productStorageController = productStorageController;
        }

        public async Task<long[]> GetRequiredUpdates()
        {
            return null;
            //return (await productStorageController.Pull<long>(ProductTypes.NewUpdatedProduct)).ToArray();
        }
    }
}
