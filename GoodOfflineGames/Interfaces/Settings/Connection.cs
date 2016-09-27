namespace Interfaces.Settings
{
    public interface IUserAgentProperty
    {
        string UserAgent { get; set; }
    }

    public interface IConnectionProperties:
        IUserAgentProperty
    {
        // ...
    }
}
