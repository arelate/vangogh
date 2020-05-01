using Interfaces.Delegates.GetFilename;
using Attributes;
using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetGameDetailsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetBinFilenameDelegate))]
        public GetGameDetailsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.GameDetails, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}