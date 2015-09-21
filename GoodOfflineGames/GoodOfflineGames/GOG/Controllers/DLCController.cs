using System;

using GOG.Interfaces;
using GOG.Model;

namespace GOG.Controllers
{
    public class DLCController: IDLCController
    {
        public void Process(
            IProductsController<Product> productsController, 
            GameDetails dlc, 
            Predicate<Product> action)
        {
            var dlcTitle = dlc.Title.Replace("DLC: ", string.Empty);

            var game = productsController.Find(dlcTitle);

            // TODO: Owned -> ownedController
            //if (game != null) game.Owned = true;
            //else
            //{
            //    // ah... I see you have discovered a hidden item
            //    // that should be ok - not all DLCs are available as product 
            //    // (e.g. special preorder bonuses and kickstarter items)
            //}

            if (dlc.DLCs != null &&
                dlc.DLCs.Count > 0)
            {
                foreach (GameDetails childDlc in dlc.DLCs)
                {
                    Process(productsController, childDlc, action);
                }
            }
        }
    }
}
