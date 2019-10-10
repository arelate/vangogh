using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Delegates.Correct;

using Interfaces.Models.Settings;
using Interfaces.Models.Entities;

using Interfaces.Status;

using Models.Directories;
using Models.Settings;

namespace Delegates.Correct
{
    public class CorrectSettingsDirectoriesAsyncDelegate : ICorrectAsyncDelegate<Settings>
    {
        public async Task<Settings> CorrectAsync(Settings settings, IStatus status)
        {
            return await Task.Run(() =>
            {
                if (settings == null)
                    throw new System.ArgumentNullException();

                var requiredDirectories = new string[] {
                    Directories.Data,
                    Directories.RecycleBin,
                    Directories.ProductImages,
                    Directories.AccountProductImages,
                    Directories.Reports,
                    Directories.Md5,
                    Directories.ProductFiles,
                    Directories.Screenshots
                };

                foreach (var requiredDirectory in requiredDirectories)
                    if (!settings.Directories.ContainsKey(requiredDirectory))
                        settings.Directories.Add(requiredDirectory, requiredDirectory);

                return settings;
            });
        }
    }
}