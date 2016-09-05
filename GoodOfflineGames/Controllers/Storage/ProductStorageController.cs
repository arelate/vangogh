using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Storage;
using Interfaces.Serialization;
using Interfaces.ProductTypes;

namespace Controllers.Storage
{
    public class ProductStorageController : IProductTypeStorageController
    {
        private IStringStorageController storageController;
        private ISerializationController<string> serializationController;

        private const string prefixTemplate = "var {0}=";
        private const string filenameTemplate = "{0}.js";

        public ProductStorageController(
            IStringStorageController storageController,
            ISerializationController<string> serializationController)
        {
            this.storageController = storageController;
            this.serializationController = serializationController;
        }

        public async Task<List<Type>> Pull<Type>(ProductTypes productType)
        {
            var prefix = string.Format(prefixTemplate, GetName(productType));
            var uri = GetFilename(productType);

            var dataString = await storageController.Pull(uri);
            var unprefixedDataString = dataString.Replace(prefix, string.Empty);

            var data = serializationController.Deserialize<List<Type>>(unprefixedDataString);
            if (data == null) data = new List<Type>();

            return data;
        }

        public async Task Push<Type>(ProductTypes productType, IList<Type> products)
        {
            var prefix = string.Format(prefixTemplate, GetName(productType));
            var uri = GetFilename(productType);

            var prefixedDataString = prefix + serializationController.Serialize(products);
            await storageController.Push(uri, prefixedDataString);
        }

        private string GetName(ProductTypes productType)
        {
            switch (productType)
            {
                case ProductTypes.AccountProduct:
                    return "accountProducts";
                case ProductTypes.Product:
                    return "products";
                case ProductTypes.Wishlisted:
                    return "wishlisted";
                case ProductTypes.NewUpdatedProduct:
                    return "updated";
                case ProductTypes.GameProductData:
                    return "gameProductData";
                case ProductTypes.ApiProduct:
                    return "apiProducts";
                case ProductTypes.GameDetails:
                    return "gameDetails";
                case ProductTypes.Screenshot:
                    return "screenshots";
                case ProductTypes.ScheduledDownload:
                    return "scheduledDownloads";
                default:
                    throw new System.NotImplementedException();
            }
        }

        private string GetFilename(ProductTypes productType)
        {
            return string.Format(filenameTemplate, GetName(productType));
        }
    }
}
