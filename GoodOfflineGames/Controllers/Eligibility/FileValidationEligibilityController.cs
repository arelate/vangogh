using System;
using System.IO;
using System.Collections.Generic;

using Interfaces.Eligibility;

using Models.Separators;

namespace Controllers.Eligibility
{
    public class FileValidationEligibilityController: IEligibilityDelegate<string>
    {
        private readonly List<string> extensionsWhitelist = new List<string>(4) {
            ".exe", // Windows
            ".bin", // Windows
            ".dmg", // Mac
            ".pkg", // Mac
            ".sh" // Linux
        };

        public bool IsEligible(string uri)
        {
            if (uri.Contains(Separators.QueryString))
                uri = uri.Substring(0, uri.IndexOf(Separators.QueryString));

            return extensionsWhitelist.Contains(Path.GetExtension(uri));
        }
    }
}
