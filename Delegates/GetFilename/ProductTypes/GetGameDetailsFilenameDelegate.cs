using Interfaces.Delegates.GetFilename;
using Interfaces.Models.Dependencies;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetGameDetailsFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            DependencyContext.Default,"Delegates.GetFilename.GetBinFilenameDelegate,Delegates")]
        public GetGameDetailsFilenameDelegate(IGetFilenameDelegate GetBinFilenameDelegate) :
            base(Filenames.GameDetails, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}