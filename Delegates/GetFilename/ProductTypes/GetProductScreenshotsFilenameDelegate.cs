using Interfaces.Delegates.GetFilename;
using Attributes;
using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductScreenshotsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetBinFilenameDelegate))]
        public GetProductScreenshotsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.ProductScreenshots, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}