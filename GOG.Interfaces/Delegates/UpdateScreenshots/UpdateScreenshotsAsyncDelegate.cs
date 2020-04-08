using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace GOG.Interfaces.Delegates.UpdateScreenshots
{
    public interface IUpdateScreenshotsAsyncDelegate<Type>
    {
        Task UpdateScreenshotsAsync(Type input);
    }
}