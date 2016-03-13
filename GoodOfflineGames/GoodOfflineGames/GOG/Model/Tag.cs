using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class Tag
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "productCount")]
        public long ProductCount { get; set; }
    }
}
