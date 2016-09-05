namespace Interfaces.ImageUri
{
    public interface IExpandUriDelegate
    {
        string ExpandUri(string partialUri);
    }

    public interface IImageUriController:
        IExpandUriDelegate
    {
        // ...
    }
}
