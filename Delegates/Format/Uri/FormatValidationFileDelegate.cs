using System.IO;

using Interfaces.Delegates.Format;
using Interfaces.Delegates.GetPath;

namespace Delegates.Format.Uri
{
    public class FormatValidationFileDelegate : IFormatDelegate<string, string>
    {
        private IGetPathDelegate getPathDelegate;

        public FormatValidationFileDelegate(IGetPathDelegate getPathDelegate)
        {
            this.getPathDelegate = getPathDelegate;
        }

        public string Format(string uri)
        {
            return getPathDelegate.GetPath(
                string.Empty,
                Path.GetFileName((uri)));
        }
    }
}
