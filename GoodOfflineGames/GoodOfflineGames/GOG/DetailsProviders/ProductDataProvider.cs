using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.Model;
using GOG.SharedModels;

namespace GOG.Providers
{
    // TODO: Unit tests

    class ProductDataProvider : 
        AbstractDetailsProvider, 
        IProductDetailsProvider<Product>
    {
        public ProductDataProvider(
            IStringGetController stringGetController,
            IStringifyController serializationController):
                base(stringGetController, serializationController)
        {
            // ...
        }

        public string Message
        {
            get
            {
                return "Updating product data for GOG.com products...";
            }
        }

        public string RequestTemplate
        {
            get
            {
                return Urls.GameProductDataPageTemplate;
            }
        }

        public IStringGetController StringGetController
        {
            get
            {
                return stringGetController;
            }
        }

        public string GetRequestDetails(Product element)
        {
            return element.Url;
        }

        public void SetDetails(Product element, string dataString)
        {
            var data = serializationController.Parse<GOGData>(dataString);
            if (data != null)
            {
                element.ProductData = data.ProductData;
            }
        }

        public bool SkipCondition(Product element)
        {
            if (element.ProductData != null)
                return true;

            if (string.IsNullOrEmpty(element.Url))
                return true;

            return false;
        }
    }
}
