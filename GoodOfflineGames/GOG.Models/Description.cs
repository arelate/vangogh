using System.Runtime.Serialization;

using GOG.Interfaces.Models;

namespace GOG.Models
{
    [DataContract]
    public class Description : IDescription
    {
        [DataMember(Name = "full")]
        public string Full { get; set; }
        [DataMember(Name = "lead")]
        public string Lead { get; set; }
    }
}
