using System.IO;

using Interfaces.Delegates.Format;
using Interfaces.Delegates.GetPath;

using Attributes;

namespace Delegates.Format.Uri
{
    public class FormatValidationFileDelegate : IFormatDelegate<string, string>
    {
        readonly IGetPathDelegate getPathDelegate;

        [Dependencies(
            "Delegates.GetPath.Json.GetValidationPathDelegate,Delegates")]
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
