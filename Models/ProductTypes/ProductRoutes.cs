using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models.ProductTypes
{
    [DataContract]
    public class ProductRoutes : ProductCore
    {
        [DataMember(Name = "routes")] public List<ProductRoutesEntry> Routes { get; set; }
    }

    [DataContract]
    public class ProductRoutesEntry
    {
        [DataMember(Name = "source")] public string Source { get; set; }
        [DataMember(Name = "destination")] public string Destination { get; set; }
    }
}