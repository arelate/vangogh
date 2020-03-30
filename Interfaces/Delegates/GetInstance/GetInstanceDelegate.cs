namespace Interfaces.Delegates.GetInstance
{
    public interface IGetInstanceDelegate<Type>
    {
        Type GetInstance();
    }
}