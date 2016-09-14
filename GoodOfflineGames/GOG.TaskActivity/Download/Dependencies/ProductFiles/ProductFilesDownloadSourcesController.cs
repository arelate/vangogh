using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.DownloadSources;

namespace GOG.TaskActivities.Download.Dependencies.ProductFiles
{
    class ProductFilesDownloadSourcesController : IDownloadSourcesController
    {
        public Task<IDictionary<long, IList<string>>> GetDownloadSources()
        {
            throw new NotImplementedException();
        }
    }
}
