using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;


using Attributes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetProductRoutesPathDelegate : GetPathDelegate
    {
        [Dependencies(
            "Delegates.GetDirectory.Root.GetDataDirectoryDelegate,Delegates",
            "Delegates.GetFilename.ProductTypes.GetProductRoutesFilenameDelegate,Delegates")]
        public GetProductRoutesPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getProductRoutesFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getProductRoutesFilenameDelegate)
        {
            // ...
        }
    }
}