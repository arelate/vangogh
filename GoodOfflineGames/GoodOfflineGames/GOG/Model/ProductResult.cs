using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class ProductsResult
    {
        // fields we won't be serializing

        // public string sortBy;
        // public int totalProducts;
        // public int productsPerPage;
        // public object contentSystemCompatibility;
        // public int moviesCount;
        // public IList<Tag> tags;
        // public int updatedProductsCount;
        // public int hiddenUpdatedProductsCount;
        // public AppliedFilters appliedFilters;
        // public bool hasHiddenProducts;

        [DataMember(Name = "products")]
        public List<Product> Products { get; set; }
        [DataMember(Name = "page")]
        public int Page { get; set; }
        [DataMember(Name = "totalPages")]
        public int TotalPages { get; set; }

        public ProductsResult()
        {
            Products = new List<Product>();
        }
    }
}
