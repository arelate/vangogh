namespace Interfaces.UpdateIdentity
{
    public interface IGetUpdateIdentityDelegate<T>
    {
        string GetUpdateIdentity(T data);
    }
}
