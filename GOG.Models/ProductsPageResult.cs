using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class ProductsPageResult : PageResult
    {
        [DataMember(Name = "products")] public Product[] Products { get; set; }
    }
}