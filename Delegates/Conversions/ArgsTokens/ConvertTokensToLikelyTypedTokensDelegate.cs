using System;
using System.Collections.Generic;
using Attributes;
using Delegates.Confirmations.ArgsTokens;
using Interfaces.Delegates.Confirmations;
using Interfaces.Delegates.Conversions;
using Models.ArgsTokens;

namespace Delegates.Conversions.ArgsTokens
{
    public class ConvertTokensToLikelyTypedTokensDelegate :
        IConvertAsyncDelegate<
            IEnumerable<string>,
            IAsyncEnumerable<(string, Tokens)>>
    {
        private IConfirmAsyncDelegate<(string, Tokens)> confirmLikelyTokenTypeDelegate;

        [Dependencies(
            typeof(ConfirmLikelyTokenTypeDelegate))]
        public ConvertTokensToLikelyTypedTokensDelegate(
            IConfirmAsyncDelegate<(string, Tokens)> confirmLikelyTokenTypeDelegate)
        {
            this.confirmLikelyTokenTypeDelegate = confirmLikelyTokenTypeDelegate;
        }

        public async IAsyncEnumerable<(string, Tokens)> ConvertAsync(
            IEnumerable<string> untypedTokens)
        {
            if (untypedTokens == null) yield break;

            var groups = new Queue<Groups>(TokensGroups.ParsingExpectations.Keys);
            var tokens = new Queue<string>(untypedTokens);

            var currentGroup = groups.Count > 0 ? groups.Dequeue() : Groups.Unknown;
            var currentToken = tokens.Count > 0 ? tokens.Dequeue() : null;

            // Determining token types makes few assumptions:
            // (1) All token type belong to one of the groups (TokensGroups.ParsingExpecations.Keys)
            // (2) Token groups are sequential - if the token doesn't match any likely type in the group
            // this and every following token can only be of the type in one of the following groups
            // (3) Detemined type can be either precise - when the exact value can be matched
            // or "likely", when we are only checking the condition (prefix).
            // (4) User-provided attribute values can't be validated other than placement order,
            // so when assuming we've got to an attribute title/values groups - 
            // anything we can't determine type for - would be considered attribute value

            while (currentToken != null)
            {
                (string Token, Tokens Type) typedToken =
                    new ValueTuple<string, Tokens>(currentToken, Tokens.Unknown);

                foreach (var type in TokensGroups.ParsingExpectations[currentGroup])
                    if (await confirmLikelyTokenTypeDelegate.ConfirmAsync((currentToken, type)))
                    {
                        typedToken.Type = type;
                        yield return typedToken;
                        break;
                    }

                // If token type was NOT determined 
                if (typedToken.Type == Tokens.Unknown)
                {
                    // Progress to the next group, if there are more groups...
                    if (groups.Count > 0)
                    {
                        currentGroup = groups.Dequeue();
                    }
                    else
                    {
                        // ...or if no more groups are available - we need to return
                        // all remaining tokens are Unknown, given assumption (2) above...
                        if (!string.IsNullOrEmpty(currentToken))
                            yield return (currentToken, Tokens.Unknown);

                        foreach (var token in tokens)
                            if (!string.IsNullOrEmpty(token))
                                yield return (token, Tokens.Unknown);

                        // ...and stop progression
                        currentToken = null;
                    }
                }

                // If the type was determined - we either:
                // - progress to the next token
                // - stop progression is there are no more token
                if (typedToken.Type != Tokens.Unknown)
                    currentToken = tokens.Count > 0 ? tokens.Dequeue() : null;
            }
        }
    }
}