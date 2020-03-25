using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;


using Attributes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetValidationResultsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            "Delegates.GetDirectory.Root.GetDataDirectoryDelegate,Delegates",
            "Delegates.GetFilename.ProductTypes.GetValidationResultsFilenameDelegate,Delegates")]
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