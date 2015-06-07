using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class ProductsResult
    {
        [DataMember(Name = "products")]
        public List<Product> Products { get; set; }
        [DataMember(Name = "page")]
        public int Page { get; set; }
        [DataMember(Name = "totalPages")]
        public int TotalPages { get; set; }

        public ProductsResult()
        {
            Products = new List<Product>();
        }

        public ProductsResult(ProductsResult otherResult): this()
        {
            this.Products = (otherResult != null) ?
                new List<Product>(otherResult.Products) :
                new List<Product>();
        }

        public ProductsResult(List<Product> products): this()
        {
            this.Products = new List<Product>(products);
        }
    }
}
