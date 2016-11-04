using System.Collections.Generic;
using System.Runtime.Serialization;

using Models.ProductCore;

namespace GOG.Models.Custom
{
    [DataContract]
    public class ProductDownloads : ProductCore
    {
        [DataMember(Name = "downloads")]
        public List<ProductDownloadEntry> Downloads { get; set; }
    }

    [DataContract]
    public class ProductDownloadEntry
    {
        [DataMember(Name = "type")]
        public ProductDownloadTypes Type { get; set; }
        [DataMember(Name = "sourceUri")]
        public string SourceUri { get; set; }
        [DataMember(Name = "resolvedUri")]
        public string ResolvedUri { get; set; }
        [DataMember(Name = "destination")]
        public string Destination { get; set; }
    }
}
