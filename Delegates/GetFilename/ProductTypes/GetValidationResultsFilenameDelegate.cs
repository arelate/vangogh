using Interfaces.Delegates.GetFilename;


using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetValidationResultsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            "Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetValidationResultsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.ValidationResults, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}