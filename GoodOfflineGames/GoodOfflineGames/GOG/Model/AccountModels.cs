using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GOG
{
    [DataContract]
    class AccountProduct
    {
        [DataMember(Name = "availability")]
        public ProductAvailability Availability { get; set; }
        [DataMember(Name = "category")]
        public string Category { get; set; }
        [DataMember(Name = "dlcCount")]
        public int DlcCount { get; set; }
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "image")]
        public string Image { get; set; }
        [DataMember(Name = "isComingSoon")]
        public bool IsComingSoon { get; set; }
        [DataMember(Name = "isGalaxyCompatible ")]
        public bool IsGalaxyCompatible { get; set; }
        [DataMember(Name = "isGame")]
        public bool IsGame { get; set; }
        [DataMember(Name = "isHidden")]
        public bool IsHidden { get; set; }
        [DataMember(Name = "isMovie")]
        public bool IsMovie { get; set; }
        [DataMember(Name = "isNew")]
        public bool IsNew { get; set; }
        [DataMember(Name = "rating")]
        public int Rating { get; set; }
        [DataMember(Name = "slug")]
        public string Slug { get; set; }
        [DataMember(Name = "tags")]
        public List<Tag> Tags { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "updates")]
        public int Updates { get; set; }
        [DataMember(Name = "url")]
        public string Url { get; set; }
        [DataMember(Name = "worksOn")]
        public ProductWorksOn WorksOn { get; set; }
    }

    [DataContract]
    class Tag
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    class AccountResults
    {
        [DataMember(Name = "contentSystemCompatibility")]
        public string ContentSystemCompatibility { get; set; }
        [DataMember(Name = "hasHiddenProducts")]
        public bool HasHiddenProducts { get; set; }
        [DataMember(Name = "moviesCount")]
        public int MoviesCount { get; set; }
        [DataMember(Name = "page")]
        public int Page { get; set; }
        [DataMember(Name = "products")]
        public List<AccountProduct> Products { get; set; }
        [DataMember(Name = "productsPerPage")]
        public int ProductsPerPage { get; set; }
        [DataMember(Name = "sortBy")]
        public string SortBy { get; set; }
        [DataMember(Name = "tags")]
        public List<Tag> Tags { get; set; }
        [DataMember(Name = "totalPages")]
        public int TotalPages { get; set; }
        [DataMember(Name = "totalProducts")]
        public int TotalProducts { get; set; }
    }
}
