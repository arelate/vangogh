using Interfaces.Creators;
using Interfaces.Controllers.Collection;

using Delegates.Convert.Requests;
using Delegates.Convert.ArgsTokens;
using Delegates.Confirm.ArgsTokens;
using Delegates.Compare.ArgsDefinitions;

using Models.ArgsDefinitions;

namespace Creators.Delegates.Convert.Requests
{
    public class ConvertArgsToRequestsDelegateCreator :
        IDelegateCreator<ConvertArgsToRequestsDelegate>
    {
        private ArgsDefinition argsDefinitions;
        private ICollectionController collectionController;

        public ConvertArgsToRequestsDelegateCreator(
            ArgsDefinition argsDefinitions,
            ICollectionController collectionController)
        {
            this.argsDefinitions = argsDefinitions;
            this.collectionController = collectionController;
        }

        public ConvertArgsToRequestsDelegate CreateDelegate()
        {
            var confirmLikelyTokenTypeDelegate =
                new ConfirmLikelyTokenTypeDelegate(
                    null,
                    collectionController);

            var convertTokensToLikelyTypedTokensDelegate =
                new ConvertTokensToLikelyTypedTokensDelegate(
                    confirmLikelyTokenTypeDelegate);

            var convertLikelyTypedTokensToTypedTokensDelegate =
                new ConvertLikelyTypedToTypedTokensDelegate(
                    argsDefinitions,
                    collectionController);

            var convertMethodsSetTokensToMethodTitleTokensDelegate =
                new ConvertMethodsSetTokensToMethodTitleTokensDelegate(
                    argsDefinitions,
                    collectionController);

            var convertTokensToTypedTokensDelegate =
                new ConvertTokensToTypedTokensDelegate(
                    convertTokensToLikelyTypedTokensDelegate,
                    convertLikelyTypedTokensToTypedTokensDelegate,
                    convertMethodsSetTokensToMethodTitleTokensDelegate);

            var convertTypedTokensToRequestsDataDelegate =
                new ConvertTypedTokensToRequestsDataDelegate();

            var methodOrderCompareDelegate =
                new MethodOrderCompareDelegate(
                    argsDefinitions.Methods,
                    collectionController);

            var convertRequestsDataToResolvedCollectionsDelegate =
                new ConvertRequestsDataToResolvedCollectionsDelegate(
                    argsDefinitions,
                    collectionController);

            var convertRequestsDataToResolvedDependenciesDelegate =
                new ConvertRequestsDataToResolvedDependenciesDelegate(
                    argsDefinitions,
                    collectionController);

            var convertRequestsDataToRequestsDelegate =
                new ConvertRequestsDataToRequestsDelegate(
                    argsDefinitions,
                    collectionController);

            var convertArgsToRequestsDelegateCreator =
                new ConvertArgsToRequestsDelegate(
                    convertTokensToTypedTokensDelegate,
                    convertTypedTokensToRequestsDataDelegate,
                    convertRequestsDataToResolvedCollectionsDelegate,
                    convertRequestsDataToResolvedDependenciesDelegate,
                    methodOrderCompareDelegate,
                    convertRequestsDataToRequestsDelegate);

            return convertArgsToRequestsDelegateCreator;
        }
    }
}