using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.SharedModels;
using GOG.Models;

namespace GOG.Providers
{
    // TODO: Unit tests

    class GameDetailsProvider :
        AbstractDetailsProvider,
        IProductDetailsProvider<Product>
    {
        public GameDetailsProvider(
            IStringGetController stringGetController,
            IStringifyController serializationController) :
                base(stringGetController, serializationController)
        {
            // ...
        }

        public string Message
        {
            get
            {
                return "Updating game details for owned products...";
            }
        }

        public string RequestTemplate
        {
            get
            {
                return Urls.AccountGameDetailsTemplate;
            }
        }

        public IStringGetController StringGetController
        {
            get
            {
                return stringGetController;
            }
        }

        public string GetRequestDetails(Product element)
        {
            return element.Id.ToString();
        }

        public void SetDetails(Product element, string dataString)
        {
            var data = serializationController.Parse<GameDetails>(dataString);
            element.GameDetails = data;
        }

        //public bool SkipCondition(Product element)
        //{
        //    return false;
        //}

        public bool SkipCondition(Product element)
        {
            // skip not owned games
            if (!element.Owned) return true;

            // skip games that already have game details and have no updates
            if (element.GameDetails != null)
                return true;

            // skip DLCs as they won't have separate game details
            if (element.ProductData != null &&
                element.ProductData.RequiredProducts != null &&
                element.ProductData.RequiredProducts.Count > 0)
                return true;

            return false;
        }
    }
}
