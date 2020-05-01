using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class SystemRequirement
    {
        [DataMember(Name = "minimum")] public string Minimum { get; set; }
        [DataMember(Name = "recommended")] public string Recommended { get; set; }
    }

    [DataContract]
    [KnownType(typeof(SystemRequirement))]
    public class SystemRequirements
    {
        [DataMember(Name = "windows")] public SystemRequirement Windows { get; set; }
        [DataMember(Name = "mac")] public SystemRequirement Mac { get; set; }
        [DataMember(Name = "linux")] public SystemRequirement Linux { get; set; }
    }
}