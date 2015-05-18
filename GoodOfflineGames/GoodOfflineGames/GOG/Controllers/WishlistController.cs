using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG
{
    public class WishlistController: ProductsResultController
    {
        public WishlistController(ProductsResult productsResult): base(productsResult)
        {

        }

        // TODO: Download and cache wishlist
        public async Task UpdateWishlisted(IConsoleController consoleController)
        {
            consoleController.Write("Updating wishlisted products...");

            var wishlistGogData = await GogDataController.RequestData<ProductsResult>(Urls.Wishlist);

            wishlistGogData.Products.ForEach(wp =>
            {
                var wishlistedProduct = productsResult.Products.Find(p => p.Id == wp.Id);
                wishlistedProduct.Wishlisted = true;

            });

            consoleController.WriteLine("DONE.");
        }
    }
}
