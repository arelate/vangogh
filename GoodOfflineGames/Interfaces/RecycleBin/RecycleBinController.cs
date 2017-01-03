namespace Interfaces.RecycleBin
{
    public interface IMoveFileToRecycleBinDelegate
    {
        void MoveFileToRecycleBin(string uri);
    }

    public interface IMoveDirectoryToRecycleBinDelegate
    {
        void MoveDirectoryToRecycleBin(string uri);
    }

    public interface IRecycleBinController:
        IMoveFileToRecycleBinDelegate,
        IMoveDirectoryToRecycleBinDelegate
    {
        // ...
    }
}
