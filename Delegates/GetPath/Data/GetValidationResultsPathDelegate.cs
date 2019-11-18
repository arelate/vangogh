using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

namespace Delegates.GetPath.Data
{
    public class GetValidationResultsPathDelegate : GetPathDelegate
    {
        public GetValidationResultsPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getValidationResultsFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getValidationResultsFilenameDelegate)
        {
            // ...
        }
    }
}