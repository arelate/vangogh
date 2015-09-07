using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Model;

namespace GOG.Interfaces
{
    public interface IDLCController
    {
        void Process(IProductsController productsController, GameDetails dlc, Predicate<Product> action);
    }
}
