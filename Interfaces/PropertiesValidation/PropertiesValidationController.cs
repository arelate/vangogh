using System;

namespace Interfaces.PropertiesValidation
{
    public interface IValidatePropertiesDelegate<Type>
    {
        Type[] ValidateProperties(params Type[] properties);
    }
}
