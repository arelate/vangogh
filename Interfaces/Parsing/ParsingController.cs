namespace Interfaces.Parsing
{
    public interface IParseDelegate<T>
    {
        T Parse(string input);
    }

    public interface IParsingController<T>:
        IParseDelegate<T>
    {
        // ...
    }
}
