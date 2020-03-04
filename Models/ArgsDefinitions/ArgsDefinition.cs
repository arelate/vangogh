using System.Runtime.Serialization;

namespace Models.ArgsDefinitions
{
    [DataContract]
    public class ArgsDefinition
    {
        [DataMember(Name = "defaultArgs")]
        public string DefaultArgs {get; set;}
        [DataMember(Name = "methods")]
        public Method[] Methods { get; set; }
        [DataMember(Name = "method-sets")]
        public MethodsSet[] MethodsSets { get; set; }
        [DataMember(Name = "collections")]
        public Collection[] Collections { get; set; }
        [DataMember(Name = "parameters")]
        public Parameter[] Parameters { get; set; }
        [DataMember(Name = "dependencies")]
        public Dependency[] Dependencies { get; set; }
    }
}
