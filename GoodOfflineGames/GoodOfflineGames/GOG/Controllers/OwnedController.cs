using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.Model;

namespace GOG.Controllers
{
    public class OwnedController: IOwnedController
    {
        private IProductsController productsController;
        private IDLCController dlcController;

        public OwnedController(IProductsController productsController, IDLCController dlcController)
        {
            this.productsController = productsController;
            this.dlcController = dlcController;
        }

        public IEnumerable<Product> GetOwned()
        {
            return productsController.Reduce(p => p.Owned);
        }

        public void MarkOwned(IEnumerable<Product> owned)
        {
            if (owned == null) return;

            foreach (var op in owned)
            {
                var game = productsController.Find(op.Id);

                if (game != null) game.Owned = true;

                // also mark DLC as owned
                if (op.GameDetails != null &&
                    op.GameDetails.DLCs != null &&
                    op.GameDetails.DLCs.Count > 0)
                {
                    foreach (var dlc in op.GameDetails.DLCs)
                    {
                        dlcController.Process(productsController, dlc, p => p.Owned = true);
                    }
                }
            }
        }
    }
}
