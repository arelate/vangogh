using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract()]
    public class Mix
    {
        [DataMember(Name = "slug")]
        public string Slug { get; set; }
        [DataMember(Name = "votes")]
        public int Votes { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "user")]
        public User User { get; set; }
    }
}
