using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract()]
    public class ApiDLCProduct
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "link")]
        public string Link { get; set; }
        [DataMember(Name = "expanded_link")]
        public string ExpandedLink { get; set; }
    }

    [DataContract()]
    public class ApiDLCs
    {
        [DataMember(Name = "products")]
        public ApiDLCProduct[] Products { get; set; }
        [DataMember(Name = "all_products_url")]
        public string AllProductsUrl { get; set; }
        [DataMember(Name = "expanded_all_products_url")]
        public string ExpandedAllProductsUrl { get; set; }

    }
}
