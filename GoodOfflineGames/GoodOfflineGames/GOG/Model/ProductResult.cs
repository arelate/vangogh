using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace GOG
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
