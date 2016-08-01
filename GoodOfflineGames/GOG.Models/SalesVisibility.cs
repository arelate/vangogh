using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    class SalesVisibility: ISalesVisibility
    {
        [DataMember(Name = "isActive")]
        public bool IsActive { get; set; }
        [DataMember(Name = "fromObject")]
        public ITimezoneDate FromObject { get; set; }
        [DataMember(Name = "from")]
        public long From { get; set; }
        [DataMember(Name = "toObject")]
        public ITimezoneDate ToObject { get; set; }
        [DataMember(Name = "to")]
        public long To { get; set; }
    }
}
