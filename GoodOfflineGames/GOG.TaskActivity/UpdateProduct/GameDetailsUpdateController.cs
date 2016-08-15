using System.Threading.Tasks;
using System.Linq;

using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.Collection;
using Interfaces.Network;
using Interfaces.Serialization;
using Interfaces.Politeness;

using GOG.Models;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.UpdateProduct
{
    public class GameDetailsUpdateController : ProductUpdateController<GameDetails, AccountProduct>
    {
        public GameDetailsUpdateController(
            IProductTypeStorageController productStorageController,
            ICollectionController collectionController,
            INetworkController networkController,
            ISerializationController<string> serializationController,
            IPolitenessController politenessController,
            ITaskReportingController taskReportingController) :
            base(productStorageController,
                collectionController,
                networkController,
                serializationController,
                politenessController,
                taskReportingController)
        {
            updateProductType = ProductTypes.GameDetails;
            listProductType = ProductTypes.AccountProduct;

            displayProductName = "game details";

            //TODO: GameDetails should be driven from AccountProducts not Products
        }

        internal override async Task<long[]> GetRequiredUpdates()
        {
            return (await productStorageController.Pull<long>(ProductTypes.NewUpdatedProduct)).ToArray();
        }

        internal override GameDetails Deserialize(string content, AccountProduct product)
        {
            var gameDetails = base.Deserialize(content, product);
            gameDetails.Id = product.Id; // gameDetails by themselves don't contain Id

            return gameDetails;
        }
    }
}
