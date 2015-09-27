using System.Collections.Generic;

using GOG.Model;
using GOG.SharedModels;
using GOG.Interfaces;

namespace GOG.Controllers
{
    // TODO: Unit tests
    public class ProductsDataController : ProductCoreController<ProductData>
    {
        private IDeserializeDelegate<string> stringDeserializeController;

        public ProductsDataController(
            IList<ProductData> productsData,
            ICollectionContainer<Product> productsCollectionContainer,
            IStringGetController stringGetDelegate,
            IDeserializeDelegate<string> stringDeserializeController) :
            base(productsData,
                productsCollectionContainer,
                stringGetDelegate)
        {
            this.stringDeserializeController = stringDeserializeController;
        }

        protected override string GetRequestTemplate()
        {
            return Urls.GameProductDataPageTemplate;
        }

        protected override string GetRequestDetails(Product product)
        {
            return product.Url;
        }

        protected override bool Skip(Product product)
        {
            var existingProductData = Find(product.Id);
            if (existingProductData != null) return true;

            if (string.IsNullOrEmpty(product.Url)) return true;

            return false;
        }

        protected override ProductData Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;

            var gogData = stringDeserializeController.Deserialize<GOGData>(data);
            return gogData != null ? gogData.ProductData : null;
        }
    }
}
