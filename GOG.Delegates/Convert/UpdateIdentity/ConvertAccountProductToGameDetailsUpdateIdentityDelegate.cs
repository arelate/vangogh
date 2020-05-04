﻿using GOG.Models;
using Interfaces.Delegates.Conversions;

namespace GOG.Delegates.Convert.UpdateIdentity
{
    public class ConvertAccountProductToGameDetailsUpdateIdentityDelegate : IConvertDelegate<AccountProduct, string>
    {
        public string Convert(AccountProduct accountProduct)
        {
            return accountProduct.Id.ToString();
        }
    }
}