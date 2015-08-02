namespace GOG.Interfaces
{
    #region Console

    public interface IReadController
    {
        string Read();
        string ReadLine();
        string ReadPrivateLine();
    }

    public interface IWriteController
    {
        void Write(string message, params object[] data);
        void WriteLine(string message, params object[] data);
    }

    public interface IConsoleController :
        IReadController,
        IWriteController
    {
        // ...
    }

    #endregion
}
