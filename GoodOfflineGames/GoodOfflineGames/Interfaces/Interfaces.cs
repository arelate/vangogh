using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace GOG.Interfaces
{
    #region Console

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

    public interface IConsoleController :
        IConsoleReadableController,
        IConsoleWritableController
    {
        // ...
    }

    #endregion

    #region Stream IO

    public interface IStreamReadableController
    {
        Stream OpenReadable(string uri);
    }

    public interface IStreamWritableController
    {
        Stream OpenWritable(string uri);
    }

    public interface IStreamController :
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

    public interface IIOController :
        IStreamController,
        IFileController,
        IDirectoryController
    {
        // ...
    }

    #endregion

    #region Credentials

    public interface IUsername
    {
        string Username { get; set; }
    }

    public interface IPassword
    {
        string Password { get; set; }
    }

    public interface ICredentials :
        IUsername,
        IPassword
    {
        // ..
    }

    #endregion

    #region Serialization

    public interface ISerializationController
    {
        string Stringify<T>(T data);
        T Parse<T>(string data);
    }

    #endregion

    #region Network

    public interface IUriController
    {
        string CombineQueryParameters(IDictionary<string, string> parameters);
        string CombineUri(string baseUri, IDictionary<string, string> parameters);
    }

    public interface IFileRequestController
    {
        Task RequestFile(string fromUri, string toUri, IStreamWritableController streamWritableController);
    }

    public interface IDataRequestController
    {
        Task<T> RequestData<T>(string uri, Dictionary<string, string> parameters = null);
    }

    public interface IStringRequestController
    {
        Task<string> RequestString(string uri, IDictionary<string, string> parameters = null);
    }

    public interface IStringPostController
    {
        Task<string> PostString(string uri, IDictionary<string, string> parameters = null, string data = null);
    }

    public interface IStringNetworkController:
        IStringRequestController,
        IStringPostController
    {
        // ... 
    }

    public interface IStringDataRequestController:
        IStringRequestController,
        IDataRequestController
    {
        // ...
    }

    #endregion
}
