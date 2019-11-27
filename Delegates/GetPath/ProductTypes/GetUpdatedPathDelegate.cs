using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Attributes;

namespace Delegates.GetPath.ProductTypes
{
    public class GetUpdatedPathDelegate : GetPathDelegate
    {
        [Dependencies(
            "Delegates.GetDirectory.Root.GetDataDirectoryDelegate,Delegates",
            "Delegates.GetFilename.ProductTypes.GetUpdatedFilenameDelegate,Delegates")]
        public GetUpdatedPathDelegate(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getUpdatedFilenameDelegate) :
            base(
                getDirectoryDelegate,
                getUpdatedFilenameDelegate)
        {
            // ...
        }
    }
}