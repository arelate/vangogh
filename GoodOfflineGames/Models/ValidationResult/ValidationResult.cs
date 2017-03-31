using System.Runtime.Serialization;

using Interfaces.ValidationResult;

namespace Models.ValidationResult
{
    [DataContract]
    public class ChunkValidation: IChunkValidation
    {
        [DataMember(Name = "from")]
        public long From { get; set; }
        [DataMember(Name = "to")]
        public long To { get; set; }
        [DataMember(Name = "expectedHash")]
        public string ExpectedHash { get; set; }
        [DataMember(Name = "actualHash")]
        public string ActualHash { get; set; }
    }

    [DataContract]
    public class FileValidation: IFileValidation
    {
        [DataMember(Name = "filename")]
        public string Filename { get; set; }
        [DataMember(Name = "validationExpected")]
        public bool ValidationExpected { get; set; }
        [DataMember(Name = "validationFileExists")]
        public bool ValidationFileExists { get; set; }
        [DataMember(Name = "validationFileIsValid")]
        public bool ValidationFileIsValid { get; set; }
        [DataMember(Name = "productFileExists")]
        public bool ProductFileExists { get; set; }
        [DataMember(Name = "filenameVerified")]
        public bool FilenameVerified { get; set; }
        [DataMember(Name = "sizeVerified")]
        public bool SizeVerified { get; set; }
        [DataMember(Name = "chunks")]
        public IChunkValidation[] Chunks { get; set; }
    }

    [DataContract]
    public class ValidationResult: ProductCore.ProductCore, IValidationResult
    {
        [DataMember(Name = "files")]
        public IFileValidation[] Files { get; set; }
    }
}
