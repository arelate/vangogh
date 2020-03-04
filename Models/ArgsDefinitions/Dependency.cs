using System.Runtime.Serialization;

namespace Models.ArgsDefinitions
{
    [DataContract]
    public class Dependency
    {
        [DataMember(Name = "method")]
        public string Method { get; set; }
        [DataMember(Name = "collections")]
        public string[] Collections { get; set; }
        [DataMember(Name = "requires")]
        public Dependency[] Requires { get; set; }
    }
}