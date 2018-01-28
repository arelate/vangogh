namespace Interfaces.Controllers.StrongTypeSerialization
{
    public interface IStrongTypeSerializeDelegate<FromType, ToType>
    {
        ToType Serialize(FromType data);
    }

    public interface IStrongTypeDeserializeDelegate<FromType, ToType>
    {
        ToType Deserialize(FromType data);
    }

    public interface IStrongTypeSerializationController<FromType, ToType>:
        IStrongTypeSerializeDelegate<FromType, ToType>,
        IStrongTypeDeserializeDelegate<ToType, FromType>
    {
        // ...
    }
}
