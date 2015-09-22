using System.Runtime.Serialization;
namespace GOG.Model
{
    [DataContract]
    public class DownloadEntry
    {
        // fields we won't be serializing

        //public string date;
        //public string downloaderUrl;


        [DataMember(Name = "manualUrl")]
        public string ManualUrl { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "size")]
        public string Size { get; set; }
        [DataMember(Name = "version")]
        public string Version { get; set; }
    }
}
