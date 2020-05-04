using System.Collections.Generic;
using Attributes;
using Delegates.Collections.ArgsDefinitions;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Models.ArgsDefinitions;
using Models.ArgsTokens;

namespace Delegates.Conversions.ArgsTokens
{
    public class ConvertMethodsSetTokensToMethodTitleTokensDelegate :
        IConvertAsyncDelegate<
            IAsyncEnumerable<(string Token, Tokens Type)>,
            IAsyncEnumerable<(string Token, Tokens Type)>>
    {
        private IGetDataAsyncDelegate<ArgsDefinition, string> getArgsDefinitionsDataFromPathAsyncDelegate;
        private IFindDelegate<MethodsSet> findMethodsSetDelegate;

        [Dependencies(
            typeof(Data.Storage.ArgsDefinitions.GetArgsDefinitionsDataFromPathAsyncDelegate),
            typeof(FindMethodsSetDelegate))]
        public ConvertMethodsSetTokensToMethodTitleTokensDelegate(
            IGetDataAsyncDelegate<ArgsDefinition, string> getArgsDefinitionsDataFromPathAsyncDelegate,
            IFindDelegate<MethodsSet> findMethodsSetDelegate)
        {
            this.getArgsDefinitionsDataFromPathAsyncDelegate = getArgsDefinitionsDataFromPathAsyncDelegate;
            this.findMethodsSetDelegate = findMethodsSetDelegate;
        }

        public async IAsyncEnumerable<(string Token, Tokens Type)> ConvertAsync(
            IAsyncEnumerable<(string Token, Tokens Type)> typedTokens)
        {
            var argsDefinitions =
                await getArgsDefinitionsDataFromPathAsyncDelegate.GetDataAsync(string.Empty);
            await foreach (var typedToken in typedTokens)
                switch (typedToken.Type)
                {
                    case Tokens.MethodsSet:
                        var titledMethodsSet = findMethodsSetDelegate.Find(
                            argsDefinitions.MethodsSets,
                            methodsSet => methodsSet.Title == typedToken.Token);
                        if (titledMethodsSet == null)
                            yield return (typedToken.Token, Tokens.Unknown);
                        else
                            foreach (var methodTitle in titledMethodsSet.Methods)
                                yield return (methodTitle, Tokens.MethodTitle);
                        break;
                    default:
                        yield return typedToken;
                        break;
                }
        }
    }
}