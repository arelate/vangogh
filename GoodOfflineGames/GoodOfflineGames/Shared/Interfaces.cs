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

    public interface IDownloadRetinaImages
    {
        bool DownloadRetinaImages { get; set; }
    }

    public interface ISettings:
        ICredentials,
        IDownloadRetinaImages
    {
        // ...
    }
}
