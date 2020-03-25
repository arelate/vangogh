using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;


using Attributes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetGameProductDataPathDelegate : GetPathDelegate
    {
        [Dependencies(
            "Delegates.GetDirectory.Root.GetDataDirectoryDelegate,Delegates",
            "Delegates.GetFilename.ProductTypes.GetGameProductDataFilenameDelegate,Delegates")]
        public GetGameProductDataPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getGameProductDataFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getGameProductDataFilenameDelegate)
        {
            // ...
        }
    }
}