using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GOG
{
    [DataContract]
    class AccountResult: PagedProductsResult
    {
        [DataMember(Name = "contentSystemCompatibility")]
        public string ContentSystemCompatibility { get; set; }
        [DataMember(Name = "hasHiddenProducts")]
        public bool HasHiddenProducts { get; set; }
        [DataMember(Name = "moviesCount")]
        public int MoviesCount { get; set; }
        [DataMember(Name = "productsPerPage")]
        public int ProductsPerPage { get; set; }
        [DataMember(Name = "sortBy")]
        public string SortBy { get; set; }
        [DataMember(Name = "tags")]
        public List<Tag> Tags { get; set; }
        [DataMember(Name = "totalProducts")]
        public int TotalProducts { get; set; }
    }
}
