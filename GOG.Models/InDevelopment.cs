using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class InDevelopment
    {
        [DataMember(Name = "active")]
        public bool Active { get; set; }
        [DataMember(Name = "until")]
        public long? Until { get; set; }
    }
}
