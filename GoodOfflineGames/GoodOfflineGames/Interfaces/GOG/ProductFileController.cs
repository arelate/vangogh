using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Model;

namespace GOG.Interfaces
{
    public interface IProductFileController
    {
        bool CheckSuccess();
        IEnumerable<long> GetIds();
        IEnumerable<string> GetFolders();
        IEnumerable<string> GetFiles(string folder);
        IList<ProductFile> Filter(IList<ProductFile> productFiles);
    }
}
