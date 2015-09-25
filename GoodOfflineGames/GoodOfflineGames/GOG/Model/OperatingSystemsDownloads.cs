using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class OperatingSystemsDownloads
    {
        [DataMember(Name = "language")]
        public string Language { get; set; }
        [DataMember(Name = "linux")]
        public List<DownloadEntry> Linux { get; set; }
        [DataMember(Name = "mac")]
        public List<DownloadEntry> Mac { get; set; }
        [DataMember(Name = "windows")]
        public List<DownloadEntry> Windows { get; set; }
    }
}
