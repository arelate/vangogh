using Interfaces.Delegates.Confirmations;

namespace GOG.Delegates.Confirm.Authorization
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