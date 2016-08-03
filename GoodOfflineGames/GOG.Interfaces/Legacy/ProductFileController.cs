using System.Collections.Generic;

using GOG.Models;

namespace GOG.Interfaces
{
    public interface IProductFileController
    {
        bool CheckSuccess();
        IEnumerable<long> GetIds();
        IEnumerable<string> GetFolders();
        IEnumerable<string> GetFiles(string folder);
        ProductFile Filter(ProductFile[] productFiles);
    }
}
