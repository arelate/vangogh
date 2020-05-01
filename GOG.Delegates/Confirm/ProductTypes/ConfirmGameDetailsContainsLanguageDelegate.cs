using Interfaces.Delegates.Collections;
using Delegates.Confirm;
using Attributes;
using Models.Separators;
using Delegates.Collections.System;

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