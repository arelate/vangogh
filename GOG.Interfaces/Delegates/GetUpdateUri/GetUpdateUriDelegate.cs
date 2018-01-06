namespace GOG.Interfaces.Delegates.GetUpdateUri
{
    public interface IGetUpdateUriDelegate<T>
    {
        string GetUpdateUri(T item);
    }
}
