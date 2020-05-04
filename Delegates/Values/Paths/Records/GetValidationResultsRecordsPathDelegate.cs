using Attributes;
using Delegates.Values.Directories.ProductTypes;
using Delegates.Values.Filenames.ProductTypes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Paths.Records
{
    public class GetValidationResultsRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetRecordsDirectoryDelegate),
            typeof(GetValidationResultsFilenameDelegate))]
        public GetValidationResultsRecordsPathDelegate(
            IGetValueDelegate<string,string> getRecordsDirectoryDelegate,
            IGetValueDelegate<string, string> getValidationResultsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getValidationResultsFilenameDelegate)
        {
            // ...
        }
    }
}