using System.Collections.Generic;
using System.Runtime.Serialization;

using Interfaces.Models.Entities;

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
        public Entity Context { get; set; }
        [DataMember(Name = "sourceUri")]
        public string SourceUri { get; set; }
        [DataMember(Name = "destination")]
        public string Destination { get; set; }
    }
}
