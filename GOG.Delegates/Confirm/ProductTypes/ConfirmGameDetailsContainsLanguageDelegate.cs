using Interfaces.Delegates.Collections;
using Attributes;
using Models.Separators;
using Delegates.Collections.System;
using Delegates.Confirmations;

namespace GOG.Delegates.Confirm.ProductTypes
{
    public class ConfirmGameDetailsContainsLanguageDelegate : ConfirmStringMatchesAllDelegate
    {
        [Dependencies(
            typeof(MapStringDelegate))]
        public ConfirmGameDetailsContainsLanguageDelegate(
            IMapDelegate<string> mapStringDelegate) :
            base(
                mapStringDelegate,
                Separators.GameDetailsDownloadsStart,
                Separators.GameDetailsDownloadsEnd)
        {
            // ...
        }
    }
}