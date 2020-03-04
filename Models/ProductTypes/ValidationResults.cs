using System.Runtime.Serialization;

using Interfaces.ValidationResults;

namespace Models.ProductTypes
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
    [KnownType(typeof(ChunkValidation))]
    public class FileValidation: IFileValidationResults
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
    [KnownType(typeof(FileValidation))]
    public class ValidationResults: ProductCore, IValidationResults
    {
        [DataMember(Name = "files")]
        public IFileValidationResults[] Files { get; set; }
    }
}
