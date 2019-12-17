using System.Runtime.Serialization;
using ProtoBuf;

using Models.ProductTypes;

namespace GOG.Models
{
    [ProtoContract, DataContract]
    public class AccountProduct: ProductCore
    {
        [ProtoMember(1),DataMember(Name = "isGalaxyCompatible")]
        public bool IsGalaxyCompatible { get; set; }
        [ProtoMember(2),DataMember(Name = "tags")]
        public string[] Tags { get; set; }
        [ProtoMember(3),DataMember(Name = "availability")]
        public Availability Availability { get; set; }
        [ProtoMember(4),DataMember(Name = "image")]
        public string Image { get; set; }
        [ProtoMember(5),DataMember(Name = "url")]
        public string Url { get; set; }
        [ProtoMember(6),DataMember(Name = "worksOn")]
        public WorksOn WorksOn { get; set; }
        [ProtoMember(7),DataMember(Name = "category")]
        public string Category { get; set; }
        [ProtoMember(8),DataMember(Name = "rating")]
        public int Rating { get; set; }
        [ProtoMember(9),DataMember(Name = "isComingSoon")]
        public bool IsComingSoon { get; set; }
        [ProtoMember(10),DataMember(Name = "isMovie")]
        public bool IsMovie { get; set; }
        [ProtoMember(11),DataMember(Name = "isGame")]
        public bool IsGame { get; set; }
        [ProtoMember(12),DataMember(Name = "slug")]
        public string Slug { get; set; }
        [ProtoMember(13),DataMember(Name = "updates")]
        public int Updates { get; set; }
        [ProtoMember(14),DataMember(Name = "isNew")]
        public bool IsNew { get; set; }
        [ProtoMember(15),DataMember(Name = "dlcCount")]
        public int DLCCount { get; set; }
        [ProtoMember(16),DataMember(Name = "releaseDate")]
        public TimezoneDate ReleaseDate { get; set; }
        [ProtoMember(17),DataMember(Name = "isBaseProductMissing")]
        public bool IsBaseProductMissing { get; set; }
        [ProtoMember(18),DataMember(Name = "isHidingDisabled")]
        public bool IsHidingDisabled { get; set; }
        [ProtoMember(19),DataMember(Name = "isInDevelopment")]
        public bool IsInDevelopment { get; set; }
        [ProtoMember(20),DataMember(Name = "isHidden")]
        public bool IsHidden { get; set; }
    }
}
