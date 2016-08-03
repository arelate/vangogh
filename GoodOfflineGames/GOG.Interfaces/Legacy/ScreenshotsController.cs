using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Reporting;

using GOG.Models;

namespace GOG.Interfaces
{

    public interface IGetScreenshotsUrisDelegate
    {
        Task<List<string>> GetScreenshotsUris(Product product, IReportUpdateDelegate reportUpdateDelegate);
    }

    public interface IScreenshotsController:
        IGetScreenshotsUrisDelegate
    {
        // ...
    }
}
