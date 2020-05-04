using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Confirmations;

namespace Delegates.Confirmations
{
    public abstract class ConfirmStringMatchesAllDelegate : IConfirmDelegate<string>
    {
        private readonly string[] matches;
        private readonly IMapDelegate<string> mapDelegate;

        public ConfirmStringMatchesAllDelegate(
            IMapDelegate<string> mapDelegate,
            params string[] matches)
        {
            this.mapDelegate = mapDelegate;
            this.matches = matches;
        }

        public bool Confirm(string entry)
        {
            var matchesAll = true;

            mapDelegate.Map(matches, match => matchesAll &= entry.Contains(match));

            return matchesAll;
        }
    }
}