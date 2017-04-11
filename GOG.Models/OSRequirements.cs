using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class OSRequirements
    {
        [DataMember(Name = "Windows")]
        public string[] Windows { get; set; }
        [DataMember(Name = "Mac")]
        public string[] Mac { get; set; }
        [DataMember(Name = "Linux")]
        public string[] Linux { get; set; }
    }
}
