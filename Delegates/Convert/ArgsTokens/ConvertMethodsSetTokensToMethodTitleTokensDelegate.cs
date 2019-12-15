using System.Collections.Generic;

using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Stash;
using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Attributes;

using Models.ArgsDefinitions;
using Models.ArgsTokens;

namespace Delegates.Convert.ArgsTokens
{
    public class ConvertMethodsSetTokensToMethodTitleTokensDelegate :
        IConvertAsyncDelegate<
            IAsyncEnumerable<(string Token, Tokens Type)>,
            IAsyncEnumerable<(string Token, Tokens Type)>>
    {
        private IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate;
        private ICollectionController collectionController;

        [Dependencies(
            "Controllers.Stash.ArgsDefinitions.ArgsDefinitionsStashController,Controllers",
            "Controllers.Collection.CollectionController,Controllers")]
        public ConvertMethodsSetTokensToMethodTitleTokensDelegate(
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionsDelegate,
            ICollectionController collectionController)
        {
            this.getArgsDefinitionsDelegate = getArgsDefinitionsDelegate;
            this.collectionController = collectionController;
        }
        public async IAsyncEnumerable<(string Token, Tokens Type)> ConvertAsync(
            IAsyncEnumerable<(string Token, Tokens Type)> typedTokens,
            IStatus status)
        {
            var argsDefinitions = await getArgsDefinitionsDelegate.GetDataAsync(status);
            await foreach (var typedToken in typedTokens)
            {
                switch (typedToken.Type)
                {
                    case Tokens.MethodsSet:
                        var titledMethodsSet = collectionController.Find(
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

}