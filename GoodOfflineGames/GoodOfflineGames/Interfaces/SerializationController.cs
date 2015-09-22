namespace GOG.Interfaces
{
    #region Serialization

    public interface ISerializeDelegate<ToType>
    {
        ToType Serialize<T>(T data);
    }

    public interface IDeserializeDelegate<FromType>
    {
        T Deserialize<T>(FromType data);
    }

    public interface ISerializationController<T>:
        ISerializeDelegate<T>,
        IDeserializeDelegate<T>
    {
        // ...
    }

    #endregion
}
