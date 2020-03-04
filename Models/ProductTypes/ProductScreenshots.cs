using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models.ProductTypes
{
    [DataContract]
    public class ProductScreenshots: ProductCore
    {
        [DataMember(Name = "uris")]
        public List<string> Uris { get; set; }
    }
}
