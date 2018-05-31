using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class ContentSystemCompatibility
    {
        [DataMember(Name = "windows")]
        public bool Windows { get; set; }
        [DataMember(Name = "osx")]
        public bool OSX { get; set; }
        [DataMember(Name = "linux")]
        public bool Linux { get; set; }
    }
}
