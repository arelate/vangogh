using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.PropertyValidation;

using Models.Directories;

namespace Controllers.PropertyValidation
{
    public class DirectoriesValidationDelegate : IValidatePropertiesAsyncDelegate<IDictionary<string, string>>
    {
        public async Task<IDictionary<string, string>> ValidatePropertiesAsync(IDictionary<string, string> settingsDirectories)
        {
            return await Task.Run(() =>
            {
                var requiredDirectories = new string[] {
                Directories.Data,
                Directories.RecycleBin,
                Directories.Images,
                Directories.Reports,
                Directories.Md5,
                Directories.ProductFiles,
                Directories.Screenshots
            };

                foreach (var requiredDirectory in requiredDirectories)
                    if (!settingsDirectories.ContainsKey(requiredDirectory))
                        settingsDirectories.Add(requiredDirectory, requiredDirectory);

                return settingsDirectories;

            });
        }
    }
}