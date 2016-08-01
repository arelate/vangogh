using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Model;
using GOG.Interfaces;

namespace GOG.Controllers
{
    public class DataConsistencyController
    {
        private IList<GameDetails> gameDetails;
        private IList<ProductData> productData;

        public DataConsistencyController(IList<GameDetails> gameDetails, IList<ProductData> productData)
        {
            this.gameDetails = gameDetails;
            this.productData = productData;
        }

        private long FindRequiredProductForProductDLC(string dlcTitle, long parent)
        {
            foreach (var data in productData)
                if (data.Title == dlcTitle &&
                    data.RequiredProducts != null &&
                    data.RequiredProducts.Count > 0)
                    foreach (var requiredProduct in data.RequiredProducts)
                        if (requiredProduct.Id == parent) return data.Id;

            return -1;
        }

        public bool Update()
        {
            bool updatedSomething = false;

            foreach (var gameDetail in gameDetails)
            {
                if (gameDetail.DLCs != null &&
                    gameDetail.DLCs.Count > 0)
                    foreach (var dlc in gameDetail.DLCs)
                    {
                        if (dlc.Id > 0) continue;

                        var requiredProduct = FindRequiredProductForProductDLC(dlc.Title, gameDetail.Id);
                        if (requiredProduct > 0)
                        {
                            dlc.Id = requiredProduct;
                            updatedSomething = true;
                        }
                    }
            }

            return updatedSomething;
        }
    }
}
