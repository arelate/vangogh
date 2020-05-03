using Attributes;
using Interfaces.Delegates.Values;

namespace Delegates.Values.Filenames.ProductTypes
{
    public class GetUpdatedFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies(
            typeof(GetBinFilenameDelegate))]
        public GetUpdatedFilenameDelegate(IGetValueDelegate<string, string> GetBinFilenameDelegate) :
            base(Models.Filenames.Filenames.Updated, GetBinFilenameDelegate)
        {
            // ...
        }
    }
}