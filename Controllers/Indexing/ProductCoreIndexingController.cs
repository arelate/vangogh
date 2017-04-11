using Interfaces.Indexing;
using Models.ProductCore;

namespace Controllers.Indexing
{
    public class ProductCoreIndexingController : IIndexingController
    {
        public long GetIndex<Type>(Type data)
        {
            return (data as ProductCore).Id;
        }
    }
}
