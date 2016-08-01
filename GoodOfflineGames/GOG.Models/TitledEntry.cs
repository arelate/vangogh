using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    class TitledEntry: ITitledEntry
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "slug")]
        public string Slug { get; set; }
    }
}
