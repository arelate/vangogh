namespace Interfaces.Controllers.Stream
{
    public interface IOpenReadableDelegate
    {
        System.IO.Stream OpenReadable(string uri);
    }

    public interface IOpenWritableDelegate
    {
        System.IO.Stream OpenWritable(string uri);
    }

    public interface IStreamController :
        IOpenReadableDelegate,
        IOpenWritableDelegate
    {
        // ...
    }
}
