using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Model;

namespace GOG.Interfaces
{
    public interface IWishlistController
    {
        Task<IEnumerable<Product>> GetAll();
        void Clear(IEnumerable<Product> products);
        void SetWishlisted(IEnumerable<Product> wishlisted);
    }
}
