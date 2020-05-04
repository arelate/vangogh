using System.Collections.Generic;
using System.Threading.Tasks;
using Attributes;
using Interfaces.Delegates.Collections;
using Interfaces.Delegates.Conversions;
using Models.ArgsTokens;
using Models.Requests;

namespace Delegates.Conversions.Requests
{
    public class ConvertArgsToRequestsDelegate : IConvertAsyncDelegate<string[], IAsyncEnumerable<Request>>
    {
        private IConvertAsyncDelegate<IEnumerable<string>, IAsyncEnumerable<(string, Tokens)>>
            convertTokensToTypedTokensDelegate;

        private IConvertDelegate<IEnumerable<(string, Tokens)>, RequestsData> convertTypedTokensToRequestsDataDelegate;

        private IConvertAsyncDelegate<RequestsData, Task<RequestsData>>
            convertRequestsDataToResolvedCollectionsDelegate;

        private IConvertAsyncDelegate<RequestsData, Task<RequestsData>>
            convertRequestsDataToResolvedDependenciesDelegate;

        private ISortAsyncDelegate<string> sortMethodsByOrderDelegate;
        private IConvertAsyncDelegate<RequestsData, IAsyncEnumerable<Request>> convertRequestsDataToRequestsDelegate;

        [Dependencies(
            typeof(ArgsTokens.ConvertTokensToTypedTokensDelegate),
            typeof(ConvertTypedTokensToRequestsDataDelegate),
            typeof(ConvertRequestsDataToResolvedCollectionsDelegate),
            typeof(ConvertRequestsDataToResolvedDependenciesDelegate),
            typeof(Delegates.Collections.Requests.SortRequestsMethodsByOrderAsyncDelegate),
            typeof(ConvertRequestsDataToRequestsDelegate))]
        public ConvertArgsToRequestsDelegate(
            IConvertAsyncDelegate<IEnumerable<string>, IAsyncEnumerable<(string, Tokens)>>
                convertTokensToTypedTokensDelegate,
            IConvertDelegate<IEnumerable<(string, Tokens)>, RequestsData> convertTypedTokensToRequestsDataDelegate,
            IConvertAsyncDelegate<RequestsData, Task<RequestsData>> convertRequestsDataToResolvedCollectionsDelegate,
            IConvertAsyncDelegate<RequestsData, Task<RequestsData>> convertRequestsDataToResolvedDependenciesDelegate,
            ISortAsyncDelegate<string> sortMethodsByOrderDelegate,
            IConvertAsyncDelegate<RequestsData, IAsyncEnumerable<Request>> convertRequestsDataToRequestsDelegate)
        {
            this.convertTokensToTypedTokensDelegate = convertTokensToTypedTokensDelegate;
            this.convertTypedTokensToRequestsDataDelegate = convertTypedTokensToRequestsDataDelegate;
            this.convertRequestsDataToResolvedCollectionsDelegate = convertRequestsDataToResolvedCollectionsDelegate;
            this.convertRequestsDataToResolvedDependenciesDelegate = convertRequestsDataToResolvedDependenciesDelegate;
            this.sortMethodsByOrderDelegate = sortMethodsByOrderDelegate;
            this.convertRequestsDataToRequestsDelegate = convertRequestsDataToRequestsDelegate;
        }

        public async IAsyncEnumerable<Request> ConvertAsync(string[] tokens)
        {
            #region Phase 1: Parse tokens into typed tokens

            var typedTokens = new List<(string, Tokens)>();
            await foreach (var typedToken in convertTokensToTypedTokensDelegate.ConvertAsync(tokens))
                typedTokens.Add(typedToken);

            #endregion

            #region Phase 2: Convert typed tokens to requests data

            var requestsData = convertTypedTokensToRequestsDataDelegate.Convert(typedTokens);

            #endregion

            #region Phase 3: If there were no collections specified for a method - add all method collections

            requestsData = await convertRequestsDataToResolvedCollectionsDelegate.ConvertAsync(requestsData);

            #endregion

            #region Phase 4: Resolve dependencies some methods, collections might have

            requestsData = await convertRequestsDataToResolvedDependenciesDelegate.ConvertAsync(requestsData);

            #endregion

            #region Phase 5: Sort methods by order

            await sortMethodsByOrderDelegate.SortAsync(requestsData.Methods);

            #endregion

            #region Phase 6: Convert requests data to requests

            await foreach (var request in convertRequestsDataToRequestsDelegate.ConvertAsync(requestsData))
                yield return request;

            #endregion
        }
    }
}