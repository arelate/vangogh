using Interfaces.IO.Stream;
using Interfaces.IO.File;
using Interfaces.IO.Directory;

namespace Interfaces.IO
{
    public interface IIOController :
        IStreamController,
        IFileController,
        IDirectoryController
    {
        // ...
    }
}
