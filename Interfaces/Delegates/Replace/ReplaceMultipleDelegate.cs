namespace Interfaces.Delegates.Replace
{
    public interface IReplaceMultipleDelegate
    {
        string ReplaceMultiple(string data, string replaceWith, params string[] findWhat);
    }
}