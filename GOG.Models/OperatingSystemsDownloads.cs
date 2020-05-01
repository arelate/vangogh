using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class OperatingSystemsDownloads
    {
        [DataMember(Name = "language")] public string Language { get; set; }
        [DataMember(Name = "linux")] public DownloadEntry[] Linux { get; set; }
        [DataMember(Name = "mac")] public DownloadEntry[] Mac { get; set; }
        [DataMember(Name = "windows")] public DownloadEntry[] Windows { get; set; }
    }
}