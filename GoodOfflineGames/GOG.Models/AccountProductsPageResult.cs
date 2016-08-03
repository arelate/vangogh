using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract()]
    public class AccountProductsPageResult: PageResult
    {
        [DataMember(Name = "totalProducts")]
        public int TotalProducts { get; set; }
        [DataMember(Name = "productsPerPage")]
        public int ProductsPerPage { get; set; }
        [DataMember(Name = "accountProducts")]
        public AccountProduct[] AccountProducts { get; set; }
        [DataMember(Name = "tags")]
        public Tag[] Tags { get; set; }
        [DataMember(Name = "updatedProductsCount")]
        public int UpdatedProductsCount { get; set; }
        [DataMember(Name = "shelfBackground")]
        public string ShelfBackground { get; set; }
        [DataMember(Name = "hasHiddenProducts")]
        public bool HasHiddenProducts { get; set; }
        [DataMember(Name = "totalHiddenProductsCount")]
        public int TotalHiddenProductsCount { get; set; }
    }
}
