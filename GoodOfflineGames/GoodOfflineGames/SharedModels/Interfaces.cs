using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GOG
{
    public interface IConsoleReadableController
    {
        string Read();
        string ReadLine();
        string ReadPrivateLine();
    }

    public interface IConsoleWritableController
    {
        void Write(string message, params object[] data);
        void WriteLine(string message, params object[] data);
    }

    public interface IConsoleController: 
        IConsoleReadableController,
        IConsoleWritableController
    {
        // ...
    }

    public interface IStreamReadableController
    {
        Stream OpenReadable(string uri);
    }

    public interface IStreamWritableController
    {
        Stream OpenWritable(string uri);
    }

    public interface IStreamController:
        IStreamReadableController,
        IStreamWritableController
    {
        // ...
    }

    public interface IFileController
    {
        bool ExistsFile(string uri);
    }

    public interface IDirectoryController
    {
        void CreateDirectory(string uri);
        bool ExistsDirectory(string uri);
    }

    public interface IIOController:
        IStreamController, 
        IFileController,
        IDirectoryController
    {
        // ...
    }

    public interface IUsername
    {
        string Username { get; set; }
    }

    public interface IPassword
    {
        string Password { get; set; }
    }

    public interface ICredentials:
        IUsername,
        IPassword
    {
        // ..
    }
}
