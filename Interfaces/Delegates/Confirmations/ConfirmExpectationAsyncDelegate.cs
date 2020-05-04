using System.Threading.Tasks;

namespace Interfaces.Delegates.Confirmations
{
    public interface IConfirmExpectationAsyncDelegate<in DataType, in ExpectationType>
    {
        Task<bool> ConfirmAsync(DataType data, ExpectationType expectation);
    } 
}