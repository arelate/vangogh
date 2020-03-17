namespace Interfaces.Delegates.Create
{
    public interface ICreateDelegate<Type, FromType>
    {
        Type Create(FromType input);
    }
}