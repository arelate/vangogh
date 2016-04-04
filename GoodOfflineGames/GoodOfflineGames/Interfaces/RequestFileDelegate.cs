using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace GOG.Interfaces
{
    public interface IRequestFileDelegate
    {
        Task<Tuple<bool, Uri>> RequestFile(
            string fromUri,
            string toUri,
            IOpenWritableDelegate openWritableDelegate,
            IFileController fileController = null,
            IProgress<double> progress = null,
            IConsoleController consoleController = null);
    }
}
