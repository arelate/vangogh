using Interfaces.Delegates.Confirmations;

namespace GOG.Delegates.Confirmations.Authorization
{
    public class ConfirmSuccessfulAuthorizationDelegate: IConfirmDelegate<string>
    {
        private const string gogData = "gogData";
        
        public bool Confirm(string data)
        {
            return data.Contains(gogData);
        }
    }
}