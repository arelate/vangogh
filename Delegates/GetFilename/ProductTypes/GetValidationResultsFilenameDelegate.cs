using Interfaces.Delegates.GetFilename;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetValidationResultsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies("Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetValidationResultsFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate) :
            base(Filenames.ValidationResults, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}