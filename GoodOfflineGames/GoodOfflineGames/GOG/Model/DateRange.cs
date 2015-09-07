using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class DateRange
    {
        [DataMember(Name = "from")]
        public long From { get; set; }
        [DataMember(Name = "fromObject")]
        public TimezoneDate FromObject { get; set; }
        [DataMember(Name = "isActive")]
        public bool IsActive { get; set; }
        [DataMember(Name = "to")]
        public long To { get; set; }
        [DataMember(Name = "toObject")]
        public TimezoneDate ToObject { get; set; }
    }
}
