namespace Interfaces.Delegates.Confirmations
{
    public interface IConfirmExpectationDelegate<in DataType, in ExpectationType>
    {
        bool Confirm(DataType data, ExpectationType expectation);
    }
}