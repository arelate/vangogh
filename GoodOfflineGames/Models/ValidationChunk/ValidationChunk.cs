using System.Runtime.Serialization;

using Interfaces.Validation;

namespace Models.ValidationChunk
{
    [DataContract]
    public class ValidationChunk: IValidationChunk
    {
        [DataMember(Name = "from")]
        public long From { get; set; }
        [DataMember(Name = "to")]
        public long To { get; set; }
        [DataMember(Name = "expectedMd5")]
        public string ExpectedMD5 { get; set; }
    }
}
