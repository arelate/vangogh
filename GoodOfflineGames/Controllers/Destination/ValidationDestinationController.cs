using System.IO;

using Interfaces.Destination;

using Models.Separators;

namespace Controllers.Destination
{
    public class ValidationDestinationController : IDestinationController
    {
        private const string validationExtension = ".xml";

        public string GetDirectory(string source)
        {
            return "_md5";
        }

        public string GetFilename(string source)
        {
            var sourceParts = source.Split(
                new string[1] { Separators.QueryString }, 
                System.StringSplitOptions.RemoveEmptyEntries);

            var filenameSansQueryString = sourceParts[0];

            var filename = Path.GetFileName(filenameSansQueryString);
            if (!filename.EndsWith(validationExtension))
                filename += validationExtension;

            return filename;
        }
    }
}
