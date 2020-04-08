using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class NamedEntry
    {
        [DataMember(Name = "name")] public string Name { get; set; }
        [DataMember(Name = "slug")] public string Slug { get; set; }
    }
}