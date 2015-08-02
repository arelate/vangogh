using System.Collections.Generic;
using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.Models;
using GOG.SharedModels;

namespace GOG.Controllers
{
    public class WishlistController: IWishlistController
    {
        private IProductsController productsController;
        private IStringGetController stringGetController;
        private IStringifyController serializationController;

        public IWriteController MessageWriteDelegate { get; set; } = null;

        public WishlistController(
            IProductsController productsController,
            IStringGetController stringGetController,
            IStringifyController serializationController)
        {
            this.productsController = productsController;
            this.stringGetController = stringGetController;
            this.serializationController = serializationController;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            if (MessageWriteDelegate != null)
            {
                MessageWriteDelegate.Write("Updating wishlisted products...");
            }

            var wishlistGogDataString = await stringGetController.GetString(Urls.Wishlist);

            var wishlistGogData = serializationController.Parse<ProductsResult>(wishlistGogDataString);

            if (MessageWriteDelegate != null)
            {
                MessageWriteDelegate.WriteLine("DONE.");
            }

            return wishlistGogData.Products;
        }

        public void Clear(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                product.Wishlisted = false;
            }
        }

        public void SetWishlisted(IEnumerable<Product> wishlisted)
        {
            foreach (var wishlistedProduct in wishlisted)
            {
                var product = productsController.Find(wishlistedProduct.Id);
                product.Wishlisted = true;
            }
        }
    }
}
