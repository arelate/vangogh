namespace Interfaces.Settings
{
    public interface IEverythingProperty
    {
        bool Everything { get; set; }
    }

    public interface IUpdateProperties:
        IEverythingProperty
    {
        // ...
    }
}
