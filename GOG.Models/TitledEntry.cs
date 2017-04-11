using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class TitledEntry
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "slug")]
        public string Slug { get; set; }
    }
}
