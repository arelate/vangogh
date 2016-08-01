using System.Collections.Generic;

using GOG.Interfaces.Models;

namespace GOG.Interfaces
{
    public interface IProductFileController
    {
        bool CheckSuccess();
        IEnumerable<long> GetIds();
        IEnumerable<string> GetFolders();
        IEnumerable<string> GetFiles(string folder);
        IProductFile Filter(IProductFile[] productFiles);
    }
}
