using System;
using System.Collections.Generic;

using Xunit;

using Interfaces.Delegates.Convert;

using Models.ArgsTokens;
using Models.Requests;

using TypedTokens = System.Collections.Generic.IEnumerable<(string Token, Models.ArgsTokens.Tokens Type)>;

namespace Delegates.Convert.Requests.Tests
{
    public class ConvertTypedTokensToRequestsDataDelegateTests
    {
        private IConvertDelegate<TypedTokens, RequestsData> convertTypedTokensToRequestsDataDelegate;

        public ConvertTypedTokensToRequestsDataDelegateTests()
        {
            this.convertTypedTokensToRequestsDataDelegate = 
                new ConvertTypedTokensToRequestsDataDelegate();
        }

        [Theory]
        [InlineData(Tokens.LikelyMethodsAbbrevation)]
        [InlineData(Tokens.LikelyParameterValue)]
        [InlineData(Tokens.MethodsSet)]
        public void ConvertTypedTokensToRequestsDataDelegateThrowsOnUnsupportedTokenTypes(Tokens tokenType)
        {
            var typedTokens = new (string, Tokens)[] {(string.Empty, tokenType)};
            Assert.Throws<NotImplementedException>(
                () => 
                convertTypedTokensToRequestsDataDelegate.Convert(typedTokens));
        }

        [Fact]
        public void ConvertTypedTokensToRequestsDataDelegateThrowsWhenParameterValuePrecedesParameterTitle()
        {
            var typedTokens = new (string, Tokens)[] {(string.Empty, Tokens.ParameterValue)};
            Assert.Throws<ArgumentException>(
                () => 
                convertTypedTokensToRequestsDataDelegate.Convert(typedTokens));            
        }

        [Theory]
        [InlineData(Tokens.MethodTitle, "1", "2", "3")]
        [InlineData(Tokens.CollectionTitle, "4", "5")]
        [InlineData(Tokens.Unknown, "6")]
        public void ConvertTypedTokensToRequestsDataDelegatePassesTitlesToCollections(Tokens tokenType, params string[] tokens)
        {
            var typedTokens = new List<(string, Tokens)>(tokens.Length);
            foreach (var token in tokens)
                typedTokens.Add((token, tokenType));

            var requestData = convertTypedTokensToRequestsDataDelegate.Convert(typedTokens);
            List<string> collection = null;
            switch (tokenType) 
            {
                case Tokens.MethodTitle:
                    collection = requestData.Methods;
                    break;
                case Tokens.CollectionTitle:
                    collection = requestData.Collections;
                    break;
                case Tokens.Unknown:
                    collection = requestData.UnknownTokens;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Assert.NotEmpty(collection);
            Assert.Equal(tokens.Length, collection.Count);
        }

        [Theory]
        [InlineData(1,2)]        
        [InlineData(2,1)]
        [InlineData(2,2)]
        public void ConvertTypedTokensToRequestsDataDelegateCollectsParameters(int parameterTitles, int parameterValues)
        {
            var typedTokens = new List<(string, Tokens)>();

            for (var tt=0; tt<parameterTitles; tt++)
            {
                typedTokens.Add((tt.ToString(), Tokens.ParameterTitle));
                for (var vv=0;vv<parameterValues; vv++)
                    typedTokens.Add((vv.ToString(), Tokens.ParameterValue));
            }

            var requestData = convertTypedTokensToRequestsDataDelegate.Convert(typedTokens);

            Assert.NotNull(requestData.Parameters);
            Assert.Equal(parameterTitles, requestData.Parameters.Count);
            foreach (var parameters in requestData.Parameters)
                Assert.Equal(parameterValues, parameters.Value.Count);
        }
    }
}