using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetValidationResultsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetValidationResultsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.ValidationResults, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}