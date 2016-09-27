using Interfaces.Settings;

namespace Interfaces.PropertiesValidation
{
    public interface IValidatePropertiesDelegate<Type>
    {
        Type ValidateProperties(Type properties);
    }

    public interface IAuthenticationPropertiesValidationController:
        IValidatePropertiesDelegate<IAuthenticationProperties>
    {
        // ...
    }

    public interface IDownloadPropertiesValidationController:
        IValidatePropertiesDelegate<IDownloadProperties>
    {
        // ...
    }
}
