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
                    Directories.Base[Entity.Data],
                    Directories.Base[Entity.RecycleBin],
                    Directories.Base[Entity.ProductImages],
                    Directories.Base[Entity.AccountProductImages],
                    Directories.Base[Entity.Reports],
                    Directories.Base[Entity.Md5],
                    Directories.Base[Entity.ProductFiles],
                    Directories.Base[Entity.Screenshots]
                };

                foreach (var requiredDirectory in requiredDirectories)
                    if (!settings.Directories.ContainsKey(requiredDirectory))
                        settings.Directories.Add(requiredDirectory, requiredDirectory);

                return settings;
            });
        }
    }
}