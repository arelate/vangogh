using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace GOG.Interfaces
{
    #region Network

    public interface IUriController
    {
        string CombineQueryParameters(IDictionary<string, string> parameters);
        string CombineUri(string baseUri, IDictionary<string, string> parameters);
    }

    public interface IFileRequestController
    {
        Task<string> RequestFile(
            string fromUri,
            string toUri,
            IStreamWritableController streamWritableController,
            IFileController fileController = null,
            IProgress<double> progress = null);
    }

    public interface IStringGetController
    {
        Task<string> GetString(string uri, IDictionary<string, string> parameters = null);
    }

    public interface IStringPostController
    {
        Task<string> PostString(string uri, IDictionary<string, string> parameters = null, string data = null);
    }

    public interface IStringNetworkController :
        IStringGetController,
        IStringPostController
    {
        // ... 
    }

    #endregion
}
