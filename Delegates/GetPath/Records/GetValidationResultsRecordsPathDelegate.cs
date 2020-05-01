using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Attributes;

namespace Delegates.GetPath.Records
{
    public class GetValidationResultsRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(Delegates.GetDirectory.ProductTypes.GetRecordsDirectoryDelegate),
            typeof(Delegates.GetFilename.ProductTypes.GetValidationResultsFilenameDelegate))]
        public GetValidationResultsRecordsPathDelegate(
            IGetDirectoryDelegate getRecordsDirectoryDelegate,
            IGetFilenameDelegate getValidationResultsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getValidationResultsFilenameDelegate)
        {
            // ...
        }
    }
}