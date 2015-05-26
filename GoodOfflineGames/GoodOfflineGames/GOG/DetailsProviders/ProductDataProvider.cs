using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.Models;
using GOG.SharedModels;

namespace GOG.Providers
{
    class ProductDataProvider : 
        AbstractDetailsProvider, 
        IProductDetailsProvider<Product>
    {
        public ProductDataProvider(
            IStringRequestController stringRequestController,
            ISerializationController serializationController):
                base(stringRequestController, serializationController)
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

        public IStringRequestController StringRequestController
        {
            get
            {
                return stringRequestController;
            }
        }

        public string GetRequestDetails(Product element)
        {
            return element.Url;
        }

        public void SetDetails(Product element, string dataString)
        {
            var data = serializationController.Parse<ProductData>(dataString);
            element.ProductData = data;
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
