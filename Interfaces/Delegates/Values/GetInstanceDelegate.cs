namespace Interfaces.Delegates.Values
{
    public interface IGetInstanceDelegate<out Type>
    {
        Type GetInstance();
    }
}