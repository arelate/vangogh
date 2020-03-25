
using Interfaces.Delegates.Map;

using Delegates.Confirm;

using Attributes;

using Models.Separators;

namespace GOG.Delegates.Confirm.ProductTypes
{
    public class ConfirmGameDetailsContainsLanguageDelegate : ConfirmStringMatchesAllDelegate
    {
        [Dependencies(
            "Delegates.Map.System.MapStringDelegate,Delegates")]
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