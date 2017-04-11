using System.Runtime.Serialization;

using Models.ProductCore;

namespace GOG.Models
{
    [DataContract()]
    public class ApiProduct: ProductCore
    {
        [DataMember(Name = "purchase_link")]
        public string PurchaseLink { get; set; }
        [DataMember(Name = "slug")]
        public string Slug { get; set; }
        [DataMember(Name = "content_system_compatibility")]
        public ContentSystemCompatibility ContentSystemCompatibility { get; set; }
        [DataMember(Name = "languages")]
        public Languages Languages { get; set; }
        [DataMember(Name = "links")]
        public Links Links { get; set; }
        [DataMember(Name = "in_development")]
        public InDevelopment InDevelopment { get; set; }
        [DataMember(Name = "is_secret")]
        public bool IsSecret { get; set; }
        [DataMember(Name = "game_type")]
        public string GameType { get; set; }
        [DataMember(Name = "is_pre_order")]
        public bool IsPreOrder { get; set; }
        [DataMember(Name = "release_date")]
        public string ReleaseDate { get; set; }
        [DataMember(Name = "images")]
        public Images Images { get; set; }
        [DataMember(Name = "dlcs")]
        public ApiDLCs DLCs { get; set; }
    }
}
