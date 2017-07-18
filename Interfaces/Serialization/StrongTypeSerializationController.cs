using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.Serialization
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
