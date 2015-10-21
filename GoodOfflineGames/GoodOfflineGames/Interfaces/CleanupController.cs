using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG.Interfaces
{
    public interface ICleanupController
    {
        int Cleanup(
            IDictionary<string, IList<string>> filesInFolders, 
            string removeToFolder, 
            IPostUpdateDelegate postUpdateDelegate = null);
    }
}
