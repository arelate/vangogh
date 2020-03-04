using System.Runtime.Serialization;

using Interfaces.Models.Properties;

namespace Models.ArgsDefinitions
{
    [DataContract]
    public class MethodsSet: ITitleProperty
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "desc")]
        public string Description { get; set; }
        [DataMember(Name = "methods")]
        public string[] Methods { get; set; }
    }
}
