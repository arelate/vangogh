using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class Availability
    {
        [DataMember(Name = "isAvailable")]
        public bool IsAvailable { get; set; }
        [DataMember(Name = "isAvailableInAccount")]
        public bool IsAvailableInAccount { get; set; }
    }
}
