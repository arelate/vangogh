namespace Interfaces.PropertyValidation
{
    public interface IValidatePropertiesDelegate<Type>
    {
        Type[] ValidateProperties(params Type[] properties);
    }
}
