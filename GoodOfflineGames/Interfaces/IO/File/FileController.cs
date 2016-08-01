using System;

namespace Interfaces.IO.File
{
    public interface IMoveDelegate
    {
        void MoveFile(string fromUri, string toUri);
    }

    public interface IFileExistsDelegate
    {
        bool FileExists(string uri);
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
        IFileExistsDelegate,
        IGetSizeDelegate,
        IGetTimestampDelegate
    {
        // ...
    }
}
