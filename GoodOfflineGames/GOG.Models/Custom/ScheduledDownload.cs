using System.Runtime.Serialization;

namespace GOG.Models.Custom
{
    [DataContract]
    public class ScheduledDownload
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "type")]
        public ScheduledDownloadTypes Type { get; set; }
        [DataMember(Name = "source")]
        public string Source { get; set; }
        [DataMember(Name = "destination")]
        public string Destination { get; set; }
    }
}
