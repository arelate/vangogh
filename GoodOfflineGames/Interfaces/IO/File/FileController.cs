using System;

namespace Interfaces.IO.File
{
    public interface IMoveDelegate
    {
        void Move(string fromUri, string toUri);
    }

    public interface IExistsDelegate
    {
        bool Exists(string uri);
    }

    public interface IGetSizeDelegate
    {
        long GetSize(string uri);
    }

    public interface IGetTimestampDelegate
    {
        DateTime GetTimestamp(string uri);
    }

    public interface IFileController :
        IMoveDelegate,
        IExistsDelegate,
        IGetSizeDelegate,
        IGetTimestampDelegate
    {
        // ...
    }
}
