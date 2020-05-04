using Attributes;
using Delegates.Collections.System;
using Delegates.Confirmations;
using Interfaces.Delegates.Collections;
using Models.Separators;

namespace GOG.Delegates.Confirmations.ProductTypes
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