using System.Collections.Generic;
using System.Runtime.Serialization;

using Models.ProductCore;

namespace GOG.Models.Custom
{
    [DataContract]
    public class ProductScreenshots: ProductCore
    {
        [DataMember(Name = "uris")]
        public List<string> Uris { get; set; }
    }
}
