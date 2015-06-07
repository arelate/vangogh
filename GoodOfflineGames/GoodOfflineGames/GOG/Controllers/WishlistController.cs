using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.Models;
using GOG.SharedModels;

namespace GOG.Controllers
{
    public class WishlistController: IDisposable
    {
        private IStringGetController stringGetController;
        private ISerializationController serializationController;

        public WishlistController(
            IStringGetController stringRequestController,
            ISerializationController serializationController)
        {
            this.stringGetController = stringRequestController;
            this.serializationController = serializationController;
        }

        public async Task<ProductsResult> RequestWishlisted(IConsoleController consoleController)
        {
            consoleController.Write("Updating wishlisted products...");

            var wishlistGogDataString = await stringGetController.GetString(Urls.Wishlist);

            var wishlistGogData = serializationController.Parse<ProductsResult>(wishlistGogDataString);

            consoleController.WriteLine("DONE.");

            return wishlistGogData;
        }

        public void Dispose()
        {
        }
    }
}
