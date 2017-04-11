namespace Interfaces.Indexing
{
    public interface IGetIndexDelegate
    {
        long GetIndex<Type>(Type data);
    }

    public interface IIndexingController:
        IGetIndexDelegate
    {
        // ...
    }
}
