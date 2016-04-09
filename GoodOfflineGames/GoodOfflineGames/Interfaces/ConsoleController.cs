using System;

namespace GOG.Interfaces
{
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
        void Write(string message, ConsoleColor color, params object[] data);
    }

    public interface IWriteLineDelegate
    {
        void WriteLine(string message, ConsoleColor color, params object[] data);
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

    public interface IDisposableConsoleController: 
        IConsoleController,
        System.IDisposable
    {
        // ...
    }
}
