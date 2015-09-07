using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class NamedEntry
    {
        // fields we won't be serializing
        
        // public string slug;

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
