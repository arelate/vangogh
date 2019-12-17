using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models.ProductDownloads
{
    [DataContract]
    public class ProductDownloads : ProductCore.ProductCore
    {
        [DataMember(Name = "downloads")]
        public List<ProductDownloadEntry> Downloads { get; set; }
    }

    [DataContract]
    public class ProductDownloadEntry
    {
        [DataMember(Name = "context")]
        public string Type { get; set; }
        [DataMember(Name = "sourceUri")]
        public string SourceUri { get; set; }
        [DataMember(Name = "destination")]
        public string Destination { get; set; }
    }
}
