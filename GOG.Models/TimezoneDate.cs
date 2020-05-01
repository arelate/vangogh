using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class TimezoneDate
    {
        [DataMember(Name = "date")] public string Date { get; set; }
        [DataMember(Name = "timezone")] public string Timezone { get; set; }
        [DataMember(Name = "timezone_type")] public int TimezoneType { get; set; }
    }
}