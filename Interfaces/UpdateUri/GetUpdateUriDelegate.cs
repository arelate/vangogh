namespace Interfaces.UpdateUri
{
    public interface IGetUpdateUriDelegate<T>
    {
        string GetUpdateUri(T item);
    }
}
