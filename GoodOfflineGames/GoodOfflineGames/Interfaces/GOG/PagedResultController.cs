using System.Collections.Generic;
using System.Threading.Tasks;

using GOG.Models;

namespace GOG.Interfaces
{
    public interface IPagedResultController
    {
        Task<IList<Product>> GetAll(string uri, IDictionary<string, string> parameters);
        Task<ProductsResult> GetOne(string uri, IDictionary<string, string> parameters, int currentPage);
        IPagedResultFilterDelegate FilterDelegate { get; set; }
        IWriteController MessageWriteDelegate { get; set; }
    }
}
