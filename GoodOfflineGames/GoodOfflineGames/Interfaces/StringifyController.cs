namespace GOG.Interfaces
{
    #region Serialization

    public interface IStringifyDelegate
    {
        string Stringify<T>(T data);
    }

    public interface IParseDelegate
    {
        T Parse<T>(string data);
    }

    public interface IStringifyController:
        IStringifyDelegate,
        IParseDelegate
    {
        // ...
    }

    #endregion
}
