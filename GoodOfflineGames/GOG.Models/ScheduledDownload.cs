using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class ScheduledDownload
    {
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "source")]
        public string Source { get; set; }
        [DataMember(Name = "destination")]
        public string Destination { get; set; }
    }
}
