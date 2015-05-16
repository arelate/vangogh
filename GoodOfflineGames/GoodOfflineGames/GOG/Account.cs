using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG
{
    class Account
    {
        private IConsoleController consoleController;

        public Account(IConsoleController consoleController)
        {
            this.consoleController = consoleController;
        }

        public async Task<AccountResult> GetAccountGames()
        {
            var accountResult = await PagedResults<AccountResult>.Request(
                Urls.AccountGetFilteredProducts,
                QueryParameters.AccountGetFilteredProducts,
                consoleController,
                "Getting account games...");

            if (accountResult != null &&
                accountResult.Products != null)
            {
                foreach (var product in accountResult.Products)
                {
                    product.Owned = true;
                }
            }

            return accountResult;
        }
    }
}
