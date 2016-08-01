using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    public class Availability: IAvailability
    {
        [DataMember(Name = "isAvailable")]
        public bool IsAvailable { get; set; }
        [DataMember(Name = "isAvailableInAccount")]
        public bool IsAvailableInAccount { get; set; }
    }
}
