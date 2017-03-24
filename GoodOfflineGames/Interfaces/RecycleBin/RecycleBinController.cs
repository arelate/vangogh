namespace Interfaces.RecycleBin
{
    public interface IMoveToRecycleBinDelegate
    {
        void MoveToRecycleBin(string uri);
    }

    public interface IRecycleBinController:
        IMoveToRecycleBinDelegate
    {
        // ...
    }
}
