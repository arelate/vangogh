namespace Interfaces.Delegates.Data
{
    public interface IDeleteDelegate<in T>
    {
        void Delete(T uri);
    }
}