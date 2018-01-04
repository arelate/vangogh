using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.UserRequested;
using Interfaces.Status;

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

        public async Task<IEnumerable<long>> EnumerateIdsAsync(IStatus status)
        {
            var parsedIds = new List<long>();

            await Task.Run(() =>
            {
                if (!IsNullOrEmpty())
                {

                    foreach (var stringId in userRequested)
                        if (long.TryParse(stringId, out var id))
                            parsedIds.Add(id);
                }
            });

            return parsedIds;
        }
    }
}
