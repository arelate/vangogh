using Interfaces.Delegates.GetFilename;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ProductTypes
{
    public class GetProductRoutesFilenameDelegate : GetFixedFilenameDelegate
    {
        [Dependencies("Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetProductRoutesFilenameDelegate(IGetFilenameDelegate getFilenameExtensionDelegate) :
            base(Filenames.ProductRoutes, getFilenameExtensionDelegate)
        {
            // ...
        }
    }
}