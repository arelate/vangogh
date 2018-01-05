using System.Threading.Tasks;

namespace Interfaces.PropertyValidation
{
    public interface IValidatePropertiesAsyncDelegate<Type>
    {
        Task<Type> ValidatePropertiesAsync(Type properties);
    }
}
