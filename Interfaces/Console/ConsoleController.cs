namespace Interfaces.Console
{
    public interface ICursorLeftProperty
    {
        int CursorLeft { get; set; }
    }

    public interface ICursorTopProperty
    {
        int CursorTop { get; set; }
    }

    public interface IReadDelegate
    {
        string Read();
    }

    public interface IReadLineDelegate
    {
        string ReadLine();
    }

    public interface IInputPasswordDelegate
    {
        string InputPassword();
    }

    public interface IWriteDelegate
    {
        void Write(string message, params object[] data);
    }

    public interface IWriteLineDelegate
    {
        void WriteLine(string message, params object[] data);
    }

    public interface IWindowWidthProperty
    {
        int WindowWidth { get; }
    }

    public interface IWindowHeightProperty
    {
        int WindowHeight { get; }
    }

    public interface IConsoleController :
        ICursorLeftProperty,
        ICursorTopProperty,
        IWindowWidthProperty,
        IWindowHeightProperty,
        IReadDelegate,
        IReadLineDelegate,
        IInputPasswordDelegate,
        IWriteDelegate,
        IWriteLineDelegate
    {
        // ...
    }
}
