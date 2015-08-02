namespace GOG.Interfaces
{
    #region Credentials

    public interface IUsername
    {
        string Username { get; set; }
    }

    public interface IPassword
    {
        string Password { get; set; }
    }

    public interface ICredentials :
        IUsername,
        IPassword
    {
        // ..
    }

    #endregion
}
