using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models.ProductScreenshots
{
    [DataContract]
    public class ProductScreenshots: ProductCore.ProductCore
    {
        [DataMember(Name = "uris")]
        public List<string> Uris { get; set; }
    }
}
