namespace Interfaces.Console
{
    public enum MessageType
    {
        Error,
        Warning,
        Success,
        Progress,
        Default
    }

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
        void Write(string message, MessageType messageType = MessageType.Default, params object[] data);
    }

    public interface IWriteLineDelegate
    {
        void WriteLine(string message, MessageType messageType = MessageType.Default, params object[] data);
    }

    public interface IConsoleController :
        IClearDelegate,
        ISetCursorPositionDelegate,
        IReadDelegate,
        IReadLineDelegate,
        IInputPasswordDelegate,
        IWriteDelegate,
        IWriteLineDelegate
    {
        // ...
    }
}
