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
    public class WishlistController
    {
        private IStringRequestController stringRequestController;
        private ISerializationController serializationController;

        public WishlistController(
            IStringRequestController stringRequestController,
            ISerializationController serializationController)
        {
            this.stringRequestController = stringRequestController;
            this.serializationController = serializationController;
        }

        public async Task<ProductsResult> RequestWishlisted(IConsoleController consoleController)
        {
            consoleController.Write("Updating wishlisted products...");

            var wishlistGogDataString = await stringRequestController.RequestString(Urls.Wishlist);

            var wishlistGogData = serializationController.Parse<ProductsResult>(wishlistGogDataString);

            consoleController.WriteLine("DONE.");

            return wishlistGogData;
        }
    }
}
