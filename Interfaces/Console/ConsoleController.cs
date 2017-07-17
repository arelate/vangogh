namespace Interfaces.Console
{
    public interface IClearDelegate
    {
        void Clear();
    }

    public interface ISetCursorPositionDelegate
    {
        void SetCursorPosition(int left, int top);
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
        IClearDelegate,
        ISetCursorPositionDelegate,
        IReadDelegate,
        IReadLineDelegate,
        IInputPasswordDelegate,
        IWriteDelegate,
        IWriteLineDelegate,
        IWindowWidthProperty,
        IWindowHeightProperty
    {
        // ...
    }
}
