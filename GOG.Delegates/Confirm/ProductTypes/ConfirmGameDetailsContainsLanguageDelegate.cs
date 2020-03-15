using Interfaces.Models.Dependencies;
using Interfaces.Delegates.Map;

using Delegates.Confirm;

using Attributes;

using Models.Separators;

namespace GOG.Delegates.Confirm.ProductTypes
{
    public class ConfirmGameDetailsContainsLanguageDelegate : ConfirmStringMatchesAllDelegate
    {
        [Dependencies(
            DependencyContext.Default,
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