using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GOG
{
    [DataContract]
    public abstract class PagedProductsResult
    {
        [DataMember(Name = "products")]
        public List<Product> Products { get; set; }
        [DataMember(Name = "page")]
        public int Page { get; set; }
        [DataMember(Name = "totalPages")]
        public int TotalPages { get; set; }

        public Product GetProductById(long id)
        {
            if (Products == null)
            {
                return null;
            }

            foreach (var product in Products)
            {
                if (product.Id == id)
                {
                    return product;
                }
            }

            return null;
        }
    }
}
