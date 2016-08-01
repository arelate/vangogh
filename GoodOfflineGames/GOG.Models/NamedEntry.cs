using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    public class NamedEntry: INamedEntry
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "slug")]
        public string Slug { get; set; }
    }
}
