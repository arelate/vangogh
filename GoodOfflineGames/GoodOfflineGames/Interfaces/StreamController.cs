using System.IO;

namespace GOG.Interfaces
{
    public interface IOpenReadableDelegate
    {
        Stream OpenReadable(string uri);
    }

    public interface IOpenWritableDelegate
    {
        Stream OpenWritable(string uri);
    }

    public interface IStreamController :
        IOpenReadableDelegate,
        IOpenWritableDelegate
    {
        // ...
    }
}
