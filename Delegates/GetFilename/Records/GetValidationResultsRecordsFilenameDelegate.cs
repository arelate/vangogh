using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.Binary
{
    public class GetValidationResultsRecordsFilenameDelegate : GetFixedFilenameDelegate
    {
        public GetValidationResultsRecordsFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate) :
            base(Filenames.ValidationResults, getBinFilenameDelegate)
        {
            // ...
        }
    }
}