using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Model;

namespace GOG.Interfaces
{

    public interface IGetScreenshotsUrisDelegate
    {
        Task<List<string>> GetScreenshotsUris(Product product, IPostUpdateDelegate postUpdateDelegate);
    }

    public interface IScreenshotsController:
        IGetScreenshotsUrisDelegate
    {
    }
}
