namespace Interfaces.PropertyValidation
{
    public interface IValidatePropertiesDelegate<Type>
    {
        Type ValidateProperties(Type properties);
    }
}
