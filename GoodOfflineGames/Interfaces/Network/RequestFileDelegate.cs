using System.Threading.Tasks;
using System;

using Interfaces.IO.Stream;
using Interfaces.Console;
using Interfaces.IO.File;

namespace Interfaces.Network
{
    public interface IRequestFileDelegate
    {
        Task<Tuple<bool, Uri>> RequestFile(
            string fromUri,
            string toUri,
            IOpenWritableDelegate openWritableDelegate,
            IFileController fileController = null,
            IDownloadProgressReportingController downloadProgressReportingController = null,
            IConsoleController consoleController = null);
    }
}
