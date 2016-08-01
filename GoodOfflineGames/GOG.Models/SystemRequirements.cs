using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    public class SystemRequirement: ISystemRequirement
    {
        [DataMember(Name = "minimum")]
        public string Minimum { get; set; }
        [DataMember(Name = "recommended")]
        public string Recommended { get; set; }
    }

    [DataContract]
    [KnownType(typeof(SystemRequirement))]
    public class SystemRequirements: ISystemRequirements
    {
        [DataMember(Name = "windows")]
        public ISystemRequirement Windows { get; set; }
        [DataMember(Name = "mac")]
        public ISystemRequirement Mac { get; set; }
        [DataMember(Name = "linux")]
        public ISystemRequirement Linux { get; set; }
    }
}
