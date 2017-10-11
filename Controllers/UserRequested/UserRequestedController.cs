using System.Collections.Generic;

using Interfaces.UserRequested;

namespace Controllers.UserRequested
{
    public class UserRequestedController : IUserRequestedController
    {
        private string[] userRequested;

        public UserRequestedController(params string[] userRequested)
        {
            this.userRequested = userRequested;
        }

        public bool IsNullOrEmpty()
        {
            return userRequested == null || userRequested.Length == 0;
        }

        public IEnumerable<long> EnumerateIds()
        {
            if (IsNullOrEmpty()) yield break;

            foreach (var stringId in userRequested)
                if (long.TryParse(stringId, out var id))
                    yield return id;
        }
    }
}
