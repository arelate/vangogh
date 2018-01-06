using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Controllers.Directory;
using Interfaces.Controllers.Data;

using Interfaces.Status;

namespace Interfaces.Settings
{
    public interface IGetSettingsAsyncDelegate
    {
        Task<ISettings> GetSettingsAsync(IStatus status);
    }

    public interface ISettingsController:
        IDataAvailableDelegate,
        ILoadAsyncDelegate,
        IGetSettingsAsyncDelegate
    {
        // ...
    }
}
