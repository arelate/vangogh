using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class AccountProduct: ProductCore
    {
        [DataMember(Name = "isGalaxyCompatible")]
        public bool IsGalaxyCompatible { get; set; }
        [DataMember(Name = "tags")]
        public Tag[] Tags { get; set; }
        [DataMember(Name = "availability")]
        public Availability Availability { get; set; }
        [DataMember(Name = "image")]
        public string Image { get; set; }
        [DataMember(Name = "url")]
        public string Url { get; set; }
        [DataMember(Name = "worksOn")]
        public WorksOn WorksOn { get; set; }
        [DataMember(Name = "category")]
        public string Category { get; set; }
        [DataMember(Name = "rating")]
        public int Rating { get; set; }
        [DataMember(Name = "isComingSoon")]
        public bool IsComingSoon { get; set; }
        [DataMember(Name = "isMovie")]
        public bool IsMovie { get; set; }
        [DataMember(Name = "isGame")]
        public bool IsGame { get; set; }
        [DataMember(Name = "slug")]
        public string Slug { get; set; }
        [DataMember(Name = "updates")]
        public int Updates { get; set; }
        [DataMember(Name = "isNew")]
        public bool IsNew { get; set; }
        [DataMember(Name = "dlcCount")]
        public int DLCCount { get; set; }
        [DataMember(Name = "releaseDate")]
        public TimezoneDate ReleaseDate { get; set; }
        [DataMember(Name = "isBaseProductMissing")]
        public bool IsBaseProductMissing { get; set; }
        [DataMember(Name = "isHidingDisabled")]
        public bool IsHidingDisabled { get; set; }
        [DataMember(Name = "isInDevelopment")]
        public bool IsInDevelopment { get; set; }
        [DataMember(Name = "isHidden")]
        public bool IsHidden { get; set; }
    }
}
