using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class DownloadEntry
    {
        [DataMember(Name = "date")] public string Date { get; set; }
        [DataMember(Name = "manualUrl")] public string ManualUrl { get; set; }
        [DataMember(Name = "downloaderUrl")] public string DownloaderUrl { get; set; }
        [DataMember(Name = "name")] public string Name { get; set; }
        [DataMember(Name = "type")] public string Type { get; set; }
        [DataMember(Name = "info")] public int Info { get; set; }
        [DataMember(Name = "size")] public string Size { get; set; }
        [DataMember(Name = "version")] public string Version { get; set; }
    }
}