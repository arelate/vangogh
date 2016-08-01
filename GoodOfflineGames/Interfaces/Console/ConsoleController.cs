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

    public interface IReadPrivateLineDelegate
    {
        string ReadPrivateLine();
    }

    public interface IWriteDelegate
    {
        void Write(string message, MessageType messageType, params object[] data);
    }

    public interface IWriteLineDelegate
    {
        void WriteLine(string message, MessageType messageType, params object[] data);
    }

    public interface IConsoleController :
        IReadDelegate,
        IReadLineDelegate,
        IReadPrivateLineDelegate,
        IWriteDelegate,
        IWriteLineDelegate
    {
        // ...
    }
}
