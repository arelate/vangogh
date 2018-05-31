using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class Images
    {
        [DataMember(Name = "background")]
        public string Background { get; set; }
        [DataMember(Name = "logo")]
        public string Logo { get; set; }
        [DataMember(Name = "icon")]
        public string Icon { get; set; }
    }
}
