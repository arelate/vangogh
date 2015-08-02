using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Models;

namespace GOG.Interfaces
{
    public interface IDLCController
    {
        void Process(IProductsController productsController, GameDetails dlc, Predicate<Product> action);
    }
}
