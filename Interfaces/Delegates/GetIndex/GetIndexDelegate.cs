namespace Interfaces.Delegates.GetIndex
{
    public interface IGetIndexDelegate<Type>
    {
        long GetIndex(Type data);
    }
}
