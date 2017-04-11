using System.IO;
using System.Linq;

using Interfaces.Expectation;

namespace Controllers.Expectation
{
    public class ValidationExpectedDelegate : IExpectedDelegate<string>
    {
        private readonly string[] extensionsWhitelist = new string[] {
            ".exe", // Windows
            ".bin", // Windows
            ".dmg", // Mac
            ".pkg", // Mac
            ".sh" // Linux
        };

        public bool Expected(string uri)
        {
            var extension = Path.GetExtension(uri);
            return extensionsWhitelist.Contains(extension);
        }
    }
}
