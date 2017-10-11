namespace Interfaces.ImageUri
{
    public interface IExpandImageUriDelegate
    {
        string ExpandImageUri(string partialUri);
    }

    public interface IGetImageUriDelegate<Type>
    {
        string GetImageUri(Type element);
    }
}
