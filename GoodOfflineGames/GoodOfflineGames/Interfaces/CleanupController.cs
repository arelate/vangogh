using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Model;

namespace GOG.Interfaces
{
    public interface ICleanupController
    {
        int Cleanup(
            string removeToFolder, 
            IPostUpdateDelegate postUpdateDelegate = null);
    }
}
