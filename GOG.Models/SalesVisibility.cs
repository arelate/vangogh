using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class SalesVisibility
    {
        [DataMember(Name = "isActive")] public bool IsActive { get; set; }
        [DataMember(Name = "fromObject")] public TimezoneDate FromObject { get; set; }
        [DataMember(Name = "from")] public long From { get; set; }
        [DataMember(Name = "toObject")] public TimezoneDate ToObject { get; set; }
        [DataMember(Name = "to")] public long To { get; set; }
    }
}