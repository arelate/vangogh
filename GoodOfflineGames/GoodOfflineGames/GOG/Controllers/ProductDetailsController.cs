using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.Models;

namespace GOG.Controllers
{
    public class ProductDetailsController
    {
        //public async Task UpdateProductDetails(IProductDetailsProvider<Product> detailsProvider, IList<Product> additionalProducts = null)
        //{
        //    consoleController.Write(detailsProvider.Message);
        //    int totalProductsUpdated = 0;

        //    var updateProducts = new List<Product>();

        //    foreach (var product in Products)
        //    {
        //        if (detailsProvider.SkipCondition(product))
        //        {
        //            continue;
        //        }

        //        updateProducts.Add(product);
        //    }

        //    if (additionalProducts != null)
        //    {
        //        var productNames = new List<string>(additionalProducts.Count);
        //        foreach (var p in additionalProducts)
        //        {
        //            productNames.Add(p.Title);
        //        }

        //        updateProducts.AddRange(Find(productNames));
        //    }

        //    foreach (var product in updateProducts)
        //    {
        //        consoleController.Write(".");

        //        var requestUri = string.Format(detailsProvider.RequestTemplate,
        //            detailsProvider.GetRequestDetails(product));
        //        var detailsString = await detailsProvider.StringGetController.GetString(requestUri);

        //        detailsProvider.SetDetails(product, detailsString);
        //        totalProductsUpdated++;
        //    }

        //    consoleController.WriteLine("Updated details for {0} products.", totalProductsUpdated);
        //}

    }
}
