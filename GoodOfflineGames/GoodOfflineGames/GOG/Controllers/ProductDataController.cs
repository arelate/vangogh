using System.Collections.Generic;

using GOG.Model;
using GOG.SharedModels;
using GOG.Interfaces;

namespace GOG.Controllers
{
    // TODO: Unit tests
    public class ProductDataController : ProductCoreController<ProductData>
    {
        private IDeserializeDelegate<string> stringDeserializeController;

        public ProductDataController(
            IList<ProductData> productsData,
            IStringGetController stringGetDelegate,
            IDeserializeDelegate<string> stringDeserializeController) :
            base(productsData, stringGetDelegate)
        {
            this.stringDeserializeController = stringDeserializeController;
        }

        protected override string GetRequestTemplate()
        {
            return Urls.GameProductDataPageTemplate;
        }

        protected override ProductData Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;

            var gogData = stringDeserializeController.Deserialize<GOGData>(data);
            return gogData != null ? gogData.ProductData : null;
        }
    }
}
