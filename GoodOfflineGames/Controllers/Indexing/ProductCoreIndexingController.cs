using Interfaces.Indexing;
using Models.ProductCore;

namespace Controllers.Indexing
{
    public class ProductCoreIndexingController : IIndexingController<ProductCore>
    {
        public long GetIndex(ProductCore data)
        {
            return data.Id;
        }
    }
}
