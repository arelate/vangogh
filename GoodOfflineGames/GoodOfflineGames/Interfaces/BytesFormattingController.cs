namespace GOG.Interfaces
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
