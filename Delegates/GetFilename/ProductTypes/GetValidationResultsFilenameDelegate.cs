using Interfaces.Delegates.GetFilename;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetValidationResultsFilenameDelegate : GetFixedFilenameDelegate
    {
        public GetValidationResultsFilenameDelegate(IGetFilenameDelegate getBinFilenameDelegate) :
            base(Filenames.ValidationResults, getBinFilenameDelegate)
        {
            // ...
        }
    }
}