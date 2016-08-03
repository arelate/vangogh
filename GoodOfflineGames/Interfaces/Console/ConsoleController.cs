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

    public interface IReadDelegate
    {
        string Read();
    }

    public interface IReadLineDelegate
    {
        string ReadLine();
    }

    public interface IPrivateReadLineDelegate
    {
        string PrivateReadLine();
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
        IReadDelegate,
        IReadLineDelegate,
        IPrivateReadLineDelegate,
        IWriteDelegate,
        IWriteLineDelegate
    {
        // ...
    }
}
