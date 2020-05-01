using System.Threading.Tasks;

namespace Interfaces.Delegates.Confirm
{
    public interface IConfirmExpectationDelegate<in DataType, in ExpectationType>
    {
        bool Confirm(DataType data, ExpectationType expectation);
    }
    
    public interface IConfirmExpectationAsyncDelegate<in DataType, in ExpectationType>
    {
        Task<bool> ConfirmAsync(DataType data, ExpectationType expectation);
    }    
}