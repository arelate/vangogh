using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class WorksOn
    {
        [DataMember(Name = "Linux")] public bool Linux { get; set; }
        [DataMember(Name = "Mac")] public bool Mac { get; set; }
        [DataMember(Name = "Windows")] public bool Windows { get; set; }
    }
}