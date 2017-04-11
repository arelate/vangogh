using System.Runtime.Serialization;

using Interfaces.Template;

namespace Models.Template
{
    [DataContract]
    public class Template: ITemplate
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "content")]
        public string Content { get; set; }
    }
}
