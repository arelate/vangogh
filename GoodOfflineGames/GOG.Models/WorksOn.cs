using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    public class WorksOn: IWorksOn
    {
        [DataMember(Name = "Linux")]
        public bool Linux { get; set; }
        [DataMember(Name = "Mac")]
        public bool Mac { get; set; }
        [DataMember(Name = "Windows")]
        public bool Windows { get; set; }
    }
}
