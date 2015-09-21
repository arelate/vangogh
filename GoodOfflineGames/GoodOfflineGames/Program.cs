using System.Collections.Generic;

using GOG.SharedControllers;
using GOG.Controllers;
using GOG.Model;
using GOG.SharedModels;
using System.Threading.Tasks;

using GOG.Interfaces;
using System;

namespace GOG
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Model variables

            IList<Product> products = null;
            IList<long> owned = null;
            IList<long> wishlisted = null;
            IList<long> updated = null;
            IList<ProductData> productsData = null;

            #endregion

            #region IO variables

            var productTypesHelper = new ProductTypesHelper();

            var filenameTemplate = "{0}.js";
            var filenames = productTypesHelper.CreateProductTypesDictionary<ProductTypes>(filenameTemplate);

            var prefixTemplate = "var {0}=";
            var prefixes = productTypesHelper.CreateProductTypesDictionary<ProductTypes>(prefixTemplate);

            #endregion

            #region Shared IO controllers

            IConsoleController consoleController = new ConsoleController();
            IIOController ioController = new IOController();
            IStorageController<string> storageController = new StorageController(ioController);
            IStringifyController jsonController = new JSONController();
            IUriController uriController = new UriController();
            IStringNetworkController networkController = new NetworkController(uriController);
            IFileRequestController fileRequestController = networkController as IFileRequestController;

            var saveLoadHelper = new SaveLoadDataHelper(storageController, jsonController);

            #endregion

            #region Loading stored data

            // Load stored data for products, owned, wishlist and queued updates
            products = saveLoadHelper.LoadData<List<Product>>(
                filenames[ProductTypes.Products],
                prefixes[ProductTypes.Products]).Result;
            products = products ?? new List<Product>();

            owned = saveLoadHelper.LoadData<List<long>>(
                filenames[ProductTypes.Owned],
                prefixes[ProductTypes.Owned]).Result;
            owned = owned ?? new List<long>();

            updated = saveLoadHelper.LoadData<List<long>>(
                filenames[ProductTypes.Updated],
                prefixes[ProductTypes.Updated]).Result;
            updated = updated ?? new List<long>();

            productsData = saveLoadHelper.LoadData<List<ProductData>>(
                filenames[ProductTypes.ProductsData],
                prefixes[ProductTypes.ProductsData]).Result;
            productsData = productsData ?? new List<ProductData>();

            #endregion

            #region GOG controllers

            var settingsController = new SettingsController(
                ioController,
                jsonController,
                consoleController);

            var authorizationController = new AuthorizationController(
                uriController,
                networkController,
                consoleController);

            var existingProductsFilter = new ExistingProductsFilter();

            var pagedResultController = new MultipageRequestController(
                    networkController, 
                    jsonController, 
                    consoleController, 
                    existingProductsFilter);

            var productsController = new ProductsController(products);

            var gogDataController = new GOGDataController(networkController);

            var ownedController = new OwnedController(owned);
            var updatedController = new UpdatedController(updated);
            var productsDataController = new ProductsDataController(productsData);
            //var gameDetailsController = new GameDetailsController(gameDetails);

            #endregion

            #region Load settings

            var settings = settingsController.Load().Result;

            #endregion

            #region Authorize on site

            if (!authorizationController.Authorize(settings).Result)
            {
                consoleController.WriteLine("Press ENTER to exit...");
                consoleController.ReadLine();
                return;
            }

            #endregion

            #region Get new products from gog.com/games

            var productsFilter = new List<long>(products.Count);
            foreach (var p in products) productsFilter.Add(p.Id);

            consoleController.Write("Requesting new products from {0}...", Urls.GamesAjaxFiltered);

            var newProducts = pagedResultController.Request(
                Urls.GamesAjaxFiltered,
                QueryParameters.GamesAjaxFiltered,
                productsFilter).Result;

            if (newProducts != null)
            {
                consoleController.Write("Got {0} new products.", newProducts.Count);

                for (var pp = newProducts.Count - 1; pp >= 0; pp--)
                    productsController.Insert(0, newProducts[pp]);
            }

            consoleController.WriteLine(string.Empty);

            #endregion

            #region Get new owned products from gog.com/account

            consoleController.Write("Requesting new products from {0}...", Urls.AccountGetFilteredProducts);

            var newOwned = pagedResultController.Request(
                Urls.AccountGetFilteredProducts,
                QueryParameters.AccountGetFilteredProducts,
                owned).Result;

            if (newOwned != null)
            {
                consoleController.Write("Got {0} new products.", newOwned.Count);

                for (var oo = newOwned.Count - 1; oo >= 0; oo--)
                    ownedController.Insert(0, newOwned[oo].Id);
            }

            consoleController.WriteLine(string.Empty);

            #endregion

            #region Get updated products

            consoleController.Write("Requesting updated products...");

            QueryParameters.AccountGetFilteredProducts["isUpdated"] = "1";
            var productUpdates = pagedResultController.Request(
                Urls.AccountGetFilteredProducts,
                QueryParameters.AccountGetFilteredProducts).Result;

            if (productUpdates != null)
            {
                consoleController.Write("Got {0} updated products.", productUpdates.Count);

                foreach (var update in productUpdates)
                    updatedController.Add(update.Id);
            }

            consoleController.WriteLine(string.Empty);

            #endregion

            #region Get wishlisted 

            consoleController.Write("Requesting wishlisted products...");

            var wishlistedString = gogDataController.GetString(Urls.Wishlist).Result;
            var wishlistedProductResult = jsonController.Parse<ProductsResult>(wishlistedString);

            if (wishlistedProductResult != null &&
                wishlistedProductResult.Products != null)
            {
                var count = wishlistedProductResult.Products.Count;

                consoleController.Write("Got {0} wishlisted products.", count);

                wishlisted = new List<long>(count);

                foreach (var wish in wishlistedProductResult.Products)
                    wishlisted.Add(wish.Id);
            }

            consoleController.WriteLine(string.Empty);

            #endregion

            #region Update product data

            var productDataRequestTemplate = Urls.GameProductDataPageTemplate;

            foreach (var p in products)
            {
                var requestUri = string.Format(productDataRequestTemplate, p.Url);
                
                // TODO: port the rest

            }

            //// update product data from game pages
            //var productDataProvider = new ProductDataProvider(gogDataController, jsonController);
            //productsResultController.UpdateProductDetails(productDataProvider).Wait();

            #endregion

            #region Update game details 

            //// update game details for all owned games
            //var gameDetailsProvider = new GameDetailsProvider(networkController, jsonController);
            //productsResultController.UpdateProductDetails(gameDetailsProvider, updatedProducts).Wait();

            #endregion

            #region Download updated product files

            var downloadDetails = new List<GameDetails>();

            //foreach (var u in updated)
            //    downloadDetails.Add(productsController.Find(u));

            var productFilesController = new ProductFilesController(
                fileRequestController,
                ioController,
                consoleController);

            productFilesController.UpdateFiles(downloadDetails).Wait();

            #endregion

            #region Update images

            var imagesController = new ImagesController(fileRequestController, ioController, consoleController);
            imagesController.Update(products).Wait();

            #endregion

            #region Saving stored data

            saveLoadHelper.SaveData(
                filenames[ProductTypes.Products], 
                products, 
                prefixes[ProductTypes.Products]).Wait();

            saveLoadHelper.SaveData(
                filenames[ProductTypes.Owned], 
                owned, 
                prefixes[ProductTypes.Owned]).Wait();

            saveLoadHelper.SaveData(
                filenames[ProductTypes.Wishlisted], 
                wishlisted, 
                prefixes[ProductTypes.Wishlisted]).Wait();

            saveLoadHelper.SaveData(
                filenames[ProductTypes.Updated], 
                updated, 
                prefixes[ProductTypes.Updated]).Wait();

            saveLoadHelper.SaveData(
                filenames[ProductTypes.ProductsData],
                productsData,
                prefixes[ProductTypes.ProductsData]).Wait();

            #endregion

            #region Deauthorize on exit

            authorizationController.Deauthorize().Wait();

            #endregion

            consoleController.WriteLine("All done. Press ENTER to quit...");
            consoleController.ReadLine();
        }
    }
}
