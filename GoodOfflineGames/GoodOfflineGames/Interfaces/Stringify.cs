namespace GOG.Interfaces
{
    #region Serialization

    public interface IStringifyController
    {
        string Stringify<T>(T data);
        T Parse<T>(string data);
    }

    #endregion
}
