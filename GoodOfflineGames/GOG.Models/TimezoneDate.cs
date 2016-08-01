using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    public class TimezoneDate: ITimezoneDate
    {
        [DataMember(Name = "date")]
        public string Date { get; set; }
        [DataMember(Name = "timezone")]
        public string Timezone { get; set; }
        [DataMember(Name = "timezone_type")]
        public int TimezoneType { get; set; }
    }
}
