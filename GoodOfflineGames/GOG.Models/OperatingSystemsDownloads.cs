using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    [KnownType(typeof(DownloadEntry))]
    public class OperatingSystemsDownloads: IOperatingSystemsDownloads
    {
        [DataMember(Name = "language")]
        public string Language { get; set; }
        [DataMember(Name = "linux")]
        public IDownloadEntry[] Linux { get; set; }
        [DataMember(Name = "mac")]
        public IDownloadEntry[] Mac { get; set; }
        [DataMember(Name = "windows")]
        public IDownloadEntry[] Windows { get; set; }
    }
}
