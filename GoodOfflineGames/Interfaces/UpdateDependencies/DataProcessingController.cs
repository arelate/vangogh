namespace Interfaces.UpdateDependencies
{
    public interface IDecodeDataDelegate
    {
        string DecodeData(string data);
    }

    public interface IDataDecodingController:
        IDecodeDataDelegate
    {
        // ...
    }
}
