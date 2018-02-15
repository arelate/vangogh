using Interfaces.Delegates.GetDirectory;

using Interfaces.Models.Entities;

using Delegates.GetDirectory;

using Models.Directories;

namespace Ghost.Factories.Delegates
{
    public static class GetDirectoryDelegateFactory
    {
        public static IGetDirectoryDelegate CreateDirectoryDelegate(
            Entity entity,
            IGetDirectoryDelegate getRootDirectoryDelegate)
        {
            return new GetRelativeDirectoryDelegate(
                Directories.Data[entity],
                getRootDirectoryDelegate);
        }
    }
}
