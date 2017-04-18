using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Status;

namespace GOG.Interfaces.UpdateScreenshots
{
    public interface IUpdateScreenshotsDelegate<Type>
    {
        Task UpdateProductScreenshots(Type input, IStatus status);
    }
}
