using System;
using System.Linq;
using System.Collections.Generic;
using Interfaces.PropertyValidation;

using Models.Directories;

namespace Controllers.PropertyValidation
{
    public class DirectoriesValidationDelegate : IValidatePropertiesDelegate<IDictionary<string, string>>
    {
        public IDictionary<string, string> ValidateProperties(IDictionary<string, string> settingsDirectories)
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
        }
    }
}