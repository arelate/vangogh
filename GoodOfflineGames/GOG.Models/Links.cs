using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract()]
    public class Links
    {
        [DataMember(Name = "purchase_link")]
        public string PurchaseLink { get; set; }
        [DataMember(Name = "product_card")]
        public string ProductCard { get; set; }
        [DataMember(Name = "support")]
        public string Support { get; set; }
        [DataMember(Name = "forum")]
        public string Forum { get; set; }
    }
}
