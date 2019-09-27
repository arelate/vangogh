using System;
using System.Collections.Generic;

using Interfaces.Delegates.Convert;

using Delegates.Convert.Requests;
using Delegates.Convert.ArgsTokens;
using Delegates.Confirm.ArgsTokens;
using Delegates.Compare.ArgsDefinitions;

using Attributes;

using Models.ArgsTokens;
using Models.Requests;

using TypedTokens = System.Collections.Generic.IEnumerable<(string Token, Models.ArgsTokens.Tokens Type)>;

namespace Delegates.Convert.Requests
{
    public class ConvertArgsToRequestsDelegate : IConvertDelegate<string[], IEnumerable<Request>>
    {
        private IConvertDelegate<IEnumerable<string>, TypedTokens> convertTokensToTypedTokensDelegate;
        private IConvertDelegate<TypedTokens, RequestsData> convertTypedTokensToRequestsDataDelegate;
        private IConvertDelegate<RequestsData, RequestsData> convertRequestsDataToResolvedCollectionsDelegate;
        private IConvertDelegate<RequestsData, RequestsData> convertRequestsDataToResolvedDependenciesDelegate;

        private IComparer<string> methodOrderCompareDelegate;
        private IConvertDelegate<RequestsData, IEnumerable<Request>> convertRequestsDataToRequestsDelegate;

        [ImplementationDependencies(
            typeof(ConvertTokensToTypedTokensDelegate),
            typeof(ConvertTypedTokensToRequestsDataDelegate),
            typeof(ConvertRequestsDataToResolvedCollectionsDelegate),
            typeof(ConvertRequestsDataToResolvedDependenciesDelegate),
            typeof(MethodOrderCompareDelegate),
            typeof(ConvertRequestsDataToRequestsDelegate))]
        public ConvertArgsToRequestsDelegate(
            IConvertDelegate<IEnumerable<string>, TypedTokens> convertTokensToTypedTokensDelegate,
            IConvertDelegate<TypedTokens, RequestsData> convertTypedTokensToRequestsDataDelegate,
            IConvertDelegate<RequestsData, RequestsData> convertRequestsDataToResolvedCollectionsDelegate,
            IConvertDelegate<RequestsData, RequestsData> convertRequestsDataToResolvedDependenciesDelegate,
            IComparer<string> methodOrderCompareDelegate,
            IConvertDelegate<RequestsData, IEnumerable<Request>> convertRequestsDataToRequestsDelegate)
        {
            this.convertTokensToTypedTokensDelegate = convertTokensToTypedTokensDelegate;
            this.convertTypedTokensToRequestsDataDelegate = convertTypedTokensToRequestsDataDelegate;
            this.convertRequestsDataToResolvedCollectionsDelegate = convertRequestsDataToResolvedCollectionsDelegate;
            this.convertRequestsDataToResolvedDependenciesDelegate = convertRequestsDataToResolvedDependenciesDelegate;
            this.methodOrderCompareDelegate = methodOrderCompareDelegate;
            this.convertRequestsDataToRequestsDelegate = convertRequestsDataToRequestsDelegate;
        }

        public IEnumerable<Request> Convert(string[] tokens)
        {
            #region Phase 1: Parse tokens into typed tokens

            var typedTokens = convertTokensToTypedTokensDelegate.Convert(tokens);

            #endregion

            #region Phase 2: Convert typed tokens to requests data

            var requestsData = convertTypedTokensToRequestsDataDelegate.Convert(typedTokens);

            #endregion

            #region Phase 3: If there were no collections specified for a method - add all method collections

            requestsData = convertRequestsDataToResolvedCollectionsDelegate.Convert(requestsData);

            #endregion

            #region Phase 4: Resolve dependencies some methods, collections might have

            requestsData = convertRequestsDataToResolvedDependenciesDelegate.Convert(requestsData);

            #endregion

            #region Phase 5: Sort methods by order

            requestsData.Methods.Sort(methodOrderCompareDelegate);

            #endregion

            #region Phase 6: Convert requests data to requests

            return convertRequestsDataToRequestsDelegate.Convert(requestsData);

            #endregion
        }
    }
}
