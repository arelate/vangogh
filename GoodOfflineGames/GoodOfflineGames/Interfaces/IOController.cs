using System;
using System.Collections.Generic;
using System.IO;

namespace GOG.Interfaces
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

    public interface IFileController:
        IMoveDelegate,
        IFileExistsDelegate,
        IGetSizeDelegate
    {
        // ...
    }

    public interface ICreateDirectoryDelegate
    {
        void CreateDirectory(string uri);
    }

    public interface IDirectoryExistsDelegate
    {
        bool DirectoryExists(string uri);
    }

    public interface IEnumerateFilesDelegate
    {
        IEnumerable<string> EnumerateFiles(string uri);
    }

    public interface IDirectoryController:
        ICreateDirectoryDelegate,
        IDirectoryExistsDelegate,
        IEnumerateFilesDelegate
    {
        // ...
    }

    public interface IIOController :
        IStreamController,
        IFileController,
        IDirectoryController
    {
        // ...
    }
}
