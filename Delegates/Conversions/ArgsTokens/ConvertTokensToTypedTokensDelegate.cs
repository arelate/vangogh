using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Conversions;
using Models.ArgsTokens;

namespace Delegates.Conversions.ArgsTokens
{
    public class ConvertTokensToTypedTokensDelegate :
        IConvertAsyncDelegate<
            IEnumerable<string>,
            IAsyncEnumerable<(string, Tokens)>>
    {
        private IConvertAsyncDelegate<IEnumerable<string>, IAsyncEnumerable<(string, Tokens)>>
            convertTokensToLikelyTypedTokensDelegate;

        private IConvertAsyncDelegate<IAsyncEnumerable<(string, Tokens)>, IAsyncEnumerable<(string, Tokens)>>
            convertLikelyTypedToTypedTokensDelegate;

        private IConvertAsyncDelegate<IAsyncEnumerable<(string, Tokens)>, IAsyncEnumerable<(string, Tokens)>>
            convertMethodsSetTokensToMethodTitleTokensDelegate;

        [Dependencies(
            typeof(ConvertTokensToLikelyTypedTokensDelegate),
            typeof(ConvertLikelyTypedToTypedTokensDelegate),
            typeof(ConvertMethodsSetTokensToMethodTitleTokensDelegate))]
        public ConvertTokensToTypedTokensDelegate(
            IConvertAsyncDelegate<IEnumerable<string>, IAsyncEnumerable<(string, Tokens)>>
                convertTokensToLikelyTypedTokensDelegate,
            IConvertAsyncDelegate<IAsyncEnumerable<(string, Tokens)>, IAsyncEnumerable<(string, Tokens)>>
                convertLikelyTypedToTypedTokensDelegate,
            IConvertAsyncDelegate<IAsyncEnumerable<(string, Tokens)>, IAsyncEnumerable<(string, Tokens)>>
                convertMethodsSetTokensToMethodTitleTokensDelegate)
        {
            this.convertTokensToLikelyTypedTokensDelegate = convertTokensToLikelyTypedTokensDelegate;
            this.convertLikelyTypedToTypedTokensDelegate = convertLikelyTypedToTypedTokensDelegate;
            this.convertMethodsSetTokensToMethodTitleTokensDelegate =
                convertMethodsSetTokensToMethodTitleTokensDelegate;
        }

        public IAsyncEnumerable<(string, Tokens)> ConvertAsync(IEnumerable<string> tokens)
        {
            // 1. Convert untyped tokens into a likely-typed tokens
            var likelyTypedTokens =
                convertTokensToLikelyTypedTokensDelegate.ConvertAsync(
                    tokens);
            // 2. Resolve likely types into specific types
            var typedTokens =
                convertLikelyTypedToTypedTokensDelegate.ConvertAsync(
                    likelyTypedTokens);
            // 3. Expand methods sets (if present)
            var expandedTypedTokens =
                convertMethodsSetTokensToMethodTitleTokensDelegate.ConvertAsync(
                    typedTokens);

            return expandedTypedTokens;
        }
    }
}