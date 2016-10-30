using System.Collections.Generic;
using System.Runtime.Serialization;

using Models.ProductCore;

namespace GOG.Models.Custom
{
    [DataContract]
    public class ScheduledDownload : ProductCore
    {
        [DataMember(Name = "downloads")]
        public List<ScheduledDownloadEntry> Downloads { get; set; }
    }

    [DataContract]
    public class ScheduledDownloadEntry
    {
        [DataMember(Name = "type")]
        public ScheduledDownloadTypes Type { get; set; }
        [DataMember(Name = "source")]
        public string Source { get; set; }
        [DataMember(Name = "destination")]
        public string Destination { get; set; }
    }
}
