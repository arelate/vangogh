namespace Interfaces.Settings
{
    public interface IUsernameProperty
    {
        string Username { get; set; }
    }

    public interface IPasswordProperty
    {
        string Password { get; set; }
    }

    public interface IAuthenticationProperties :
        IUsernameProperty,
        IPasswordProperty
    {
        // ..
    }
}
