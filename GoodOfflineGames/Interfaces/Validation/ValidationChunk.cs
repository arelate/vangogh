namespace Interfaces.Validation
{
    public interface IFromProperty
    {
        long From { get; set; }
    }

    public interface IToProperty
    {
        long To { get; set; }
    }

    public interface IExpectedMD5Property
    {
        string ExpectedMD5 { get; set; }
    }

    public interface IValidationChunk:
        IFromProperty,
        IToProperty,
        IExpectedMD5Property
    {
        // ...
    }
}
