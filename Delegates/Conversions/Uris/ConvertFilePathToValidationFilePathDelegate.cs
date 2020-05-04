using System.IO;
using Attributes;
using Delegates.Values.Paths.Json;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Values;

namespace Delegates.Conversions.Uris
{
    public class ConvertFilePathToValidationFilePathDelegate : IConvertDelegate<string, string>
    {
        private readonly IGetValueDelegate<string,(string Directory,string Filename)> getValidationPathDelegate;

        [Dependencies(
            typeof(GetValidationPathDelegate))]
        public ConvertFilePathToValidationFilePathDelegate(
            IGetValueDelegate<string,(string Directory,string Filename)> getValidationPathDelegate)
        {
            this.getValidationPathDelegate = getValidationPathDelegate;
        }

        public string Convert(string filePath)
        {
            return getValidationPathDelegate.GetValue((
                string.Empty,
                Path.GetFileName(filePath)));
        }
    }
}