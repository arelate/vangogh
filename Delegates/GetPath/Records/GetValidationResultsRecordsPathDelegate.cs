using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Records
{
    public class GetValidationResultsRecordsPathDelegate : GetPathDelegate
    {
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