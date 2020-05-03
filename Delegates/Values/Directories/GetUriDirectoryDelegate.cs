using System;
using System.IO;
using Interfaces.Delegates.Values;
using Models.Separators;

namespace Delegates.Values.Directories
{
    public abstract class GetUriDirectoryDelegate : IGetValueDelegate<string,string>
    {
        private readonly IGetValueDelegate<string,string> baseDirectoryDelegate;

        public GetUriDirectoryDelegate(IGetValueDelegate<string,string> baseDirectoryDelegate)
        {
            this.baseDirectoryDelegate = baseDirectoryDelegate;
        }

        public string GetValue(string source)
        {
            var directory = string.Empty;

            if (!string.IsNullOrEmpty(source))
            {
                var uriParts = source.Split(
                    new string[] {Separators.UriPart},
                    StringSplitOptions.RemoveEmptyEntries);

                directory = uriParts.Length >= 2 ? uriParts[uriParts.Length - 2] : source;
            }

            var baseDirectory = string.Empty;

            if (baseDirectoryDelegate != null)
                baseDirectory = baseDirectoryDelegate.GetValue(string.Empty);

            return Path.Combine(baseDirectory, directory);
        }
    }
}