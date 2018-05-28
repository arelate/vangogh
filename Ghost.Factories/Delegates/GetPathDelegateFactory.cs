using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetPath;
using Interfaces.Delegates.GetFilename;

using Interfaces.Models.Entities;

using Delegates.GetPath;

namespace Ghost.Factories.Delegates
{
    public static class GetPathDelegateFactory
    {
        public static IGetPathDelegate CreatePathDelegate(
            Entity entity,
            IGetDirectoryDelegate getRootDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate)
        {
            return new GetPathDelegate(
                GetDirectoryDelegateFactory.CreateDirectoryDelegate(
                    entity,
                    getRootDirectoryDelegate),
                getFilenameDelegate);
        }
    }
}
