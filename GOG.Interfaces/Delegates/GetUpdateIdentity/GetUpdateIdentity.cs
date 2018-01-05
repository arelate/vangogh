namespace GOG.Interfaces.Delegates.GetUpdateIdentity
{
    public interface IGetUpdateIdentityDelegate<T>
    {
        string GetUpdateIdentity(T data);
    }
}
