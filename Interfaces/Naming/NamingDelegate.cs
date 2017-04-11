namespace Interfaces.Naming
{
    public interface IGetNameDelegate
    {
        string GetName(params string[] nameParts);
    }
}
