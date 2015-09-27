using System;
using System.Collections.Generic;

using GOG.Model;
using GOG.SharedModels;
using GOG.Interfaces;
using Newtonsoft.Json;

namespace GOG.Controllers
{
    // TODO: Unit tests
    public class GameDetailsController : ProductCoreController<GameDetails>
    {
        private ICollectionController<long> ownedController;
        private ICollectionController<long> updatedController;
        private IFindDelegate<long, ProductData> productDataFindDelegate;
        private IDeserializeDelegate<string> stringDeserializeController;
        private ICollection<string> supportedLanguages;

        public GameDetailsController(
            IList<GameDetails> gameDetails,
            ICollectionContainer<Product> productsCollectionContainer,
            IStringGetController stringGetDelegate,
            IDeserializeDelegate<string> stringDeserializeController,
            ICollectionController<long> ownedController,
            ICollectionController<long> updatedController,
            IFindDelegate<long, ProductData> productDataFindDelegate,
            ICollection<string> supportedLanguages) :
            base(gameDetails,
                productsCollectionContainer,
                stringGetDelegate)
        {
            this.ownedController = ownedController;
            this.updatedController = updatedController;
            this.productDataFindDelegate = productDataFindDelegate;
            this.stringDeserializeController = stringDeserializeController;
            this.supportedLanguages = supportedLanguages;
        }

        protected override string GetRequestTemplate()
        {
            return Urls.AccountGameDetailsTemplate;
        }

        protected override string GetRequestDetails(Product product)
        {
            return product.Id.ToString();
        }

        protected override bool Skip(Product product)
        {
            if (!ownedController.Contains(product.Id)) return true;

            if (!updatedController.Contains(product.Id))
            {
                var existingGameDetails = Find(product.Id);
                if (existingGameDetails != null) return true;

                var existingProductData = productDataFindDelegate.Find(product.Id);
                if (existingProductData != null &&
                    existingProductData.RequiredProducts != null &&
                    existingProductData.RequiredProducts.Count > 0)
                    return true;
            }

            return false;
        }

        private void ExpandDynamicDownloads(ref GameDetails details)
        {
            if (details == null) return;

            details.LanguageDownloads = new List<OperatingSystemsDownloads>();

            foreach (var entry in details.DynamicDownloads)
            {
                if (entry.Length != 2)
                    throw new InvalidOperationException("Unsupported data format");

                var language = entry[0];

                if (!supportedLanguages.Contains(language)) continue;

                string downloadsString = JsonConvert.SerializeObject(entry[1]);
                var downloads = stringDeserializeController.Deserialize<OperatingSystemsDownloads>(
                    downloadsString);

                downloads.Language = language;
                details.LanguageDownloads.Add(downloads);
            }

            details.DynamicDownloads = null;

            if (details.DLCs == null) return;

            for (var ii = 0; ii < details.DLCs.Count; ii++)
            {
                var dlc = details.DLCs[ii];
                ExpandDynamicDownloads(ref dlc);
            }
        }

        protected override GameDetails Deserialize(string data)
        {
            // GameDetails are complicated as GOG.com currently serves mixed type array
            // where first entry is string "Language" and next entries are downloads
            // so in order to overcome this we do it like this:
            // 1) use Json.NET to deserialize with downloads mapped to dynamic[][] collection
            // 2) walk through DynamicDownloads collection
            // 3) for each language we expand second collection item (that is operating system downloads)
            // 4) for each DLC we recursively expand DynamicDownloads
            // 5) we nullify DynamicDownloads after expansion to allow further serialization

            // TODO: keep an eye on further changes that might allow to simplify this

            GameDetails gameDetails = JsonConvert.DeserializeObject<GameDetails>(data);

            ExpandDynamicDownloads(ref gameDetails);

            return gameDetails;
        }
    }
}
