using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    class OSRequirements: IOSRequirements
    {
        [DataMember(Name = "Windows")]
        public string[] Windows { get; set; }
        [DataMember(Name = "Mac")]
        public string[] Mac { get; set; }
        [DataMember(Name = "Linux")]
        public string[] Linux { get; set; }
    }
}
