namespace Interfaces.ValidationResults
{
    public interface IFromProperty
    {
        long From { get; set; }
    }

    public interface IToProperty
    {
        long To { get; set; }
    }

    public interface IExpectedHashProperty
    {
        string ExpectedHash { get; set; }
    }

    public interface IActualHashProperty
    {
        string ActualHash { get; set; }
    }

    public interface IChunkValidation:
        IFromProperty,
        IToProperty,
        IExpectedHashProperty,
        IActualHashProperty
    {
        // ...
    }

    public interface IFilenameProperty
    {
        string Filename { get; set; }
    }

    public interface IValidationExpectedProperty
    {
        bool ValidationExpected { get; set; }
    }

    public interface IValidationFileExistsProperty
    {
        bool ValidationFileExists { get; set; }
    }

    public interface IValidationFileIsValidProperty
    {
        bool ValidationFileIsValid { get; set; }
    }

    public interface IProductFileExistsProperty
    {
        bool ProductFileExists { get; set; }
    }

    public interface IFilenameVerifiedProperty
    {
        bool FilenameVerified { get; set; }
    }

    public interface ISizeVerifiedProperty
    {
        bool SizeVerified { get; set; }
    }

    public interface IChunksProperty
    {
        IChunkValidation[] Chunks { get; set; }
    }

    public interface IFileValidationResults:
        IFilenameProperty,
        IValidationExpectedProperty,
        IValidationFileExistsProperty,
        IValidationFileIsValidProperty,
        IProductFileExistsProperty,
        IFilenameVerifiedProperty,
        ISizeVerifiedProperty,
        IChunksProperty
    {
        // ...
    }

    public interface IValidationResults
    {
        IFileValidationResults[] Files { get; set; }
    }
}
