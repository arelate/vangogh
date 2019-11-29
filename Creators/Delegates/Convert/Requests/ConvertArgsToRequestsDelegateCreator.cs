using Interfaces.Creators;
using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Instances;

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
        private IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionDelegate;
        private ICollectionController collectionController;
        private readonly IInstancesController instancesController;

        public ConvertArgsToRequestsDelegateCreator(
            ArgsDefinition argsDefinitions,
            IGetDataAsyncDelegate<ArgsDefinition> getArgsDefinitionDelegate,
            ICollectionController collectionController,
            IInstancesController instancesController)
        {
            this.argsDefinitions = argsDefinitions;
            this.getArgsDefinitionDelegate = getArgsDefinitionDelegate;
            this.collectionController = collectionController;
            this.instancesController = instancesController;
        }

        public ConvertArgsToRequestsDelegate CreateDelegate()
        {
            // var confirmLikelyTokenTypeDelegate = dependenciesController.GetInstance(
            //    typeof(ConfirmLikelyTokenTypeDelegate)) 
            //     as ConfirmLikelyTokenTypeDelegate;

            var convertTokensToLikelyTypedTokensDelegate = instancesController.GetInstance(
                typeof(ConvertTokensToLikelyTypedTokensDelegate))
                as ConvertTokensToLikelyTypedTokensDelegate;

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