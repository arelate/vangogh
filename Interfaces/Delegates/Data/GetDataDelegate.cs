namespace Interfaces.Delegates.Data
{
    public interface IGetDataDelegate<out T>
    {
        T GetData(string uri = null);
    }
}