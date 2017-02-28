using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.FileDownload;
using Interfaces.TaskStatus;

namespace GOG.Controllers.FileDownload
{
    public class ValidationDownloadFromSourceDelegate: IDownloadFileFromSourceDelegate
    {
        public Task DownloadFileFromSourceAsync(string sourceUri, string destination, ITaskStatus taskStatus)
        {
            throw new NotImplementedException();
        }
    }
}
