using System.IO;
using Attributes;
using Delegates.Values.Filenames;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Values;

namespace Delegates.Conversions.Uris
{
    public class ConvertUriToValidationFileUriDelegate : IConvertDelegate<string, string>
    {
        private readonly IGetValueDelegate<string, string> getValidationFilenameDelegate;
        private readonly IConvertDelegate<string, string> convertSessionUriToUriSansSessionDelegate;

        [Dependencies(
            typeof(GetValidationFilenameDelegate),
            typeof(ConvertSessionUriToUriSansSessionDelegate))]
        public ConvertUriToValidationFileUriDelegate(
            IGetValueDelegate<string, string> getValidationFilenameDelegate,
            IConvertDelegate<string, string> convertSessionUriToUriSansSessionDelegate)
        {
            this.getValidationFilenameDelegate = getValidationFilenameDelegate;
            this.convertSessionUriToUriSansSessionDelegate = convertSessionUriToUriSansSessionDelegate;
        }

        public string Convert(string sourceUri)
        {
            var sourceUriSansSession = convertSessionUriToUriSansSessionDelegate.Convert(sourceUri);
            var sourceFilename = Path.GetFileName(sourceUriSansSession);
            var validationFilename = getValidationFilenameDelegate.GetValue(sourceFilename);

            return sourceUri.Replace(sourceFilename, validationFilename);
        }
    }
}