namespace Interfaces.Indexing
{
    public interface IIndexDelegate<Type>
    {
        long GetIndex(Type data);
    }

    public interface IIndexingController<Type>:
        IIndexDelegate<Type>
    {
        // ...
    }
}
