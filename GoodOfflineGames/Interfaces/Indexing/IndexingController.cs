namespace Interfaces.Indexing
{
    public interface IIndexDelegate
    {
        long GetIndex<Type>(Type data);
    }

    public interface IIndexingController:
        IIndexDelegate
    {
        // ...
    }
}
