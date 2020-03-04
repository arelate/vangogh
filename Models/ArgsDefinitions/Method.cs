using System.Runtime.Serialization;

using Interfaces.Models.Properties;

namespace Models.ArgsDefinitions
{
    [DataContract]
    public class Method: ITitleProperty
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "desc")]
        public string Description { get; set; }
        [DataMember(Name = "order")]
        public int Order { get; set; }
        [DataMember(Name = "hidden")]
        public bool Hidden { get; set; }
        [DataMember(Name = "collections")]
        public string[] Collections { get; set; }
        [DataMember(Name = "parameters")]
        public string[] Parameters { get; set; }
    }
}