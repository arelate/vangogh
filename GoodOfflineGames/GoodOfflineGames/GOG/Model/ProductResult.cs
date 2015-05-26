using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class ProductsResult
    {
        [DataMember(Name = "products")]
        public List<Product> Products { get; set; } = new List<Product>();
        [DataMember(Name = "page")]
        public int Page { get; set; }
        [DataMember(Name = "totalPages")]
        public int TotalPages { get; set; }
    }
}
