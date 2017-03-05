namespace Interfaces.Expectation
{
    public interface IExpectedDelegate<T>
    {
        bool Expected(T data);
    }
}
