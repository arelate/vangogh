using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class Recommendations
    {
        [DataMember(Name = "firstPage")] public Product[] FirstPage { get; set; }
        [DataMember(Name = "all")] public Product[] All { get; set; }
    }
}