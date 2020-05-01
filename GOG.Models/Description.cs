using System.Runtime.Serialization;

namespace GOG.Models
{
    [DataContract]
    public class Description
    {
        [DataMember(Name = "full")] public string Full { get; set; }
        [DataMember(Name = "lead")] public string Lead { get; set; }
    }
}