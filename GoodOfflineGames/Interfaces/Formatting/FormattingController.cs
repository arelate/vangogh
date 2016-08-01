namespace Interfaces.Formatting
{
    public interface IFormatDelegate
    {
        string Format(long value);
    }

    public interface IFormattingController:
        IFormatDelegate
    {
        // ...
    }
}
