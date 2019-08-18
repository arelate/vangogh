using System;
using System.Collections.Generic;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Confirm;

using Models.ArgsTokens;

using TypedTokens = System.Collections.Generic.IEnumerable<(string Token, Models.ArgsTokens.Tokens Type)>;

namespace Delegates.Convert.ArgsTokens
{
    public class ConvertTokensToTypedTokensDelegate : 
        IConvertDelegate<IEnumerable<string>, TypedTokens>
    {
        private IConvertDelegate<IEnumerable<string>, TypedTokens> convertTokensToLikelyTypedTokensDelegate;
        private IConvertDelegate<TypedTokens, TypedTokens> convertLikelyTypedToTypedTokensDelegate;
        private IConvertDelegate<TypedTokens, TypedTokens> convertMethodsSetTokensToMethodTitleTokensDelegate;

        public ConvertTokensToTypedTokensDelegate(
            IConvertDelegate<IEnumerable<string>, TypedTokens> convertTokensToLikelyTypedTokensDelegate,
            IConvertDelegate<TypedTokens, TypedTokens> convertLikelyTypedToTypedTokensDelegate,
            IConvertDelegate<TypedTokens, TypedTokens> convertMethodsSetTokensToMethodTitleTokensDelegate)
        {
            this.convertTokensToLikelyTypedTokensDelegate = convertTokensToLikelyTypedTokensDelegate;
            this.convertLikelyTypedToTypedTokensDelegate = convertLikelyTypedToTypedTokensDelegate;
            this.convertMethodsSetTokensToMethodTitleTokensDelegate = convertMethodsSetTokensToMethodTitleTokensDelegate;
        }
        public TypedTokens Convert(IEnumerable<string> tokens)
        {
            // 1. Convert untyped tokens into a likely-typed tokens
            var likelyTypedTokens = 
                convertTokensToLikelyTypedTokensDelegate.Convert(
                    tokens);
            // 2. Resolve likely types into specific types
            var typedTokens = 
                convertLikelyTypedToTypedTokensDelegate.Convert(
                    likelyTypedTokens);
            // 3. Expand methods sets (if present)
            var expandedTypedTokens = 
                convertMethodsSetTokensToMethodTitleTokensDelegate.Convert(
                    typedTokens);

            return expandedTypedTokens;
        }
    }
}