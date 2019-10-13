using System.IO;
using System.Linq;

using Interfaces.Delegates.Confirm;

using Models.Extensions;

namespace Delegates.Confirm
{
    public class ConfirmValidationExpectedDelegate : IConfirmDelegate<string>
    {
        readonly string[] extensionsWhitelist = {
            Extensions.EXE, // Windows
            Extensions.BIN, // Windows
            Extensions.DMG, // Mac
            Extensions.PKG, // Mac
            Extensions.SH // Linux
        };

        public bool Confirm(string uri)
        {
            var extension = Path.GetExtension(uri);
            return extensionsWhitelist.Contains(extension);
        }
    }
}