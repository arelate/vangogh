using System.Collections.Generic;

using Interfaces.Controllers.Collection;
using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Models.ArgsDefinitions;
using Models.ArgsTokens;

namespace Delegates.Convert.ArgsTokens
{
    public class ConvertMethodsSetTokensToMethodTitleTokensDelegate :
        IConvertAsyncDelegate<
            IAsyncEnumerable<(string Token, Tokens Type)>, 
            IAsyncEnumerable<(string Token, Tokens Type)>>
    {
        private ArgsDefinition argsDefinition;
        private ICollectionController collectionController;

        public ConvertMethodsSetTokensToMethodTitleTokensDelegate(
            ArgsDefinition argsDefinition,
            ICollectionController collectionController)
        {
            this.argsDefinition = argsDefinition;
            this.collectionController = collectionController;
        }
        public async IAsyncEnumerable<(string Token, Tokens Type)> ConvertAsync(
            IAsyncEnumerable<(string Token, Tokens Type)> typedTokens, 
            IStatus status)
        {
            await foreach (var typedToken in typedTokens)
            {
                switch (typedToken.Type)
                {
                    case Tokens.MethodsSet:
                        var titledMethodsSet = collectionController.Find(
                            argsDefinition.MethodsSets,
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

}