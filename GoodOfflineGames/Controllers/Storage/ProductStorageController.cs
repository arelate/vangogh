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

        private const string prefixTemplate = "var {0}s=";
        private const string uriTemplate = "{0}s.js";
        private string prefix;
        private string uri;

        public ProductStorageController(
            IStringStorageController storageController,
            ISerializationController<string> serializationController)
        {
            this.storageController = storageController;
            this.serializationController = serializationController;
        }

        public async Task<IList<Type>> Pull<Type>(ProductTypes productType)
        {
            prefix = string.Format(prefixTemplate, productType.ToString().ToLower());
            uri = string.Format(uriTemplate, productType.ToString().ToLower());

            var dataString = await storageController.Pull(uri);
            var unprefixedDataString = dataString.Replace(prefix, string.Empty);

            return serializationController.Deserialize<IList<Type>>(unprefixedDataString);
        }

        public async Task Push<Type>(ProductTypes productType, IList<Type> products)
        {
            prefix = string.Format(prefixTemplate, productType.ToString().ToLower());
            uri = string.Format(uriTemplate, productType.ToString().ToLower());

            var prefixedDataString = prefix + serializationController.Serialize(products);
            await storageController.Push(uri, prefixedDataString);
        }
    }
}
