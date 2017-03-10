using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.UpdateUri;
using Interfaces.ProductTypes;

using Models.Uris;

namespace Controllers.UpdateUri
{
    public class ProductTypesGetUpdateUriDelegate : IGetUpdateUriDelegate<ProductTypes>
    {
        public string GetUpdateUri(ProductTypes productType)
        {
            switch (productType)
            {
                case ProductTypes.Product:
                    return Uris.Paths.Games.AjaxFiltered;
                case ProductTypes.AccountProduct:
                    return Uris.Paths.Account.GetFilteredProducts;
                case ProductTypes.Screenshot: // screenshots use the same page as game product data
                case ProductTypes.GameProductData:
                    return Uris.Paths.GameProductData.ProductTemplate;
                case ProductTypes.ApiProduct:
                    return Uris.Paths.Api.ProductTemplate;
                case ProductTypes.GameDetails:
                    return Uris.Paths.Account.GameDetailsTemplate;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
