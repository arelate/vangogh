using Interfaces.Delegates.GetFilename;


using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductDownloadsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            "Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetProductDownloadsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.ProductDownloads, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}