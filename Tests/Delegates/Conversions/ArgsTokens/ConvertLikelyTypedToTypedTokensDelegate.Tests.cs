using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delegates.Conversions.ArgsTokens;
using Interfaces.Delegates.Conversions;
using Models.ArgsTokens;
using Tests.TestDelegates.Conversions.Types;
using Xunit;

namespace Tests.Delegates.Conversions.ArgsTokens
{
    public class ConvertLikelyTypedToTypedTokensDelegateTests
    {
        private readonly IConvertAsyncDelegate<IEnumerable<string>, IAsyncEnumerable<(string, Tokens)>>
            convertTokensToLikelyTypedTokensDelegate;

        private readonly IConvertAsyncDelegate<IAsyncEnumerable<(string, Tokens)>, IAsyncEnumerable<(string, Tokens)>>
            convertLikelyTypedToTypedTokensDelegate;

        public ConvertLikelyTypedToTypedTokensDelegateTests()
        {
            convertTokensToLikelyTypedTokensDelegate = DelegatesInstances.TestConvertTypeToInstanceDelegate.Convert(
                    typeof(ConvertTokensToLikelyTypedTokensDelegate))
                as ConvertTokensToLikelyTypedTokensDelegate;

            convertLikelyTypedToTypedTokensDelegate = DelegatesInstances.TestConvertTypeToInstanceDelegate.Convert(
                    typeof(ConvertLikelyTypedToTypedTokensDelegate))
                as ConvertLikelyTypedToTypedTokensDelegate;
        }

        private async Task<List<(string, Tokens)>> ConvertTokensToTypedTokens(params string[] tokens)
        {
            var likelyTypedTokes = convertTokensToLikelyTypedTokensDelegate.ConvertAsync(tokens);

            var typedTokens = new List<(string, Tokens)>();

            await foreach (var typedToken in
                convertLikelyTypedToTypedTokensDelegate.ConvertAsync(likelyTypedTokes))
                typedTokens.Add(typedToken);

            return typedTokens;
        }

        [Theory]
        [InlineData("-upd")]
        [InlineData("-updx")] // ends with unknown token
        [InlineData("-xupd")] // starts with unknown token
        public async void ConvertLikelyMethodAbbrevationToMethodTitleTokens(string token)
        {
            var typedTokens = await ConvertTokensToTypedTokens(token);

            var methodTitlesCount = 0;
            foreach (var typedToken in typedTokens)
            {
                if (typedToken.Item2 == Tokens.Unknown) continue;
                Assert.Equal(Tokens.MethodTitle, typedToken.Item2);
                methodTitlesCount++;
            }

            Assert.Equal(3, methodTitlesCount);
        }

        [Theory]
        [InlineData("-")]
        [InlineData(null)]
        [InlineData("")]
        public async void ConvertLikelyMethodAbbrevationHandlesInvalidInputs(string token)
        {
            var typedTokens = await ConvertTokensToTypedTokens(token);

            Assert.NotNull(typedTokens);
            Assert.Empty(typedTokens);
        }

        [Theory]
        [InlineData("--id")]
        // [InlineData("--arbitrarystring")] // starts with two dashes
        public async void ConvertParameterTitleToParameterTitleTrimLeadingDash(string token)
        {
            var typedTokens = await ConvertTokensToTypedTokens(token);

            foreach (var typedToken in typedTokens)
                Assert.Equal(Tokens.ParameterTitle, typedToken.Item2);
        }

        [Theory]
        [InlineData("--os", "windows")]
        [InlineData("--os", "osx")]
        [InlineData("--os", "linux")]
        [InlineData("--lang", "en")]
        [InlineData("--id", "arbitrarystring")] // parameter that doesn't have values
        public async void ConvertParameterTitleAndLikelyParameterValuesSucceeds(string parameter, string likelyValue)
        {
            var typedTokens = await ConvertTokensToTypedTokens(parameter, likelyValue);

            Assert.Equal(2, typedTokens.Count());
            Assert.Equal(Tokens.ParameterTitle, typedTokens.ElementAt(0).Item2);
            Assert.Equal(Tokens.ParameterValue, typedTokens.ElementAt(1).Item2);
        }

        [Theory]
        [InlineData("--os", "bsd")]
        [InlineData("--lang", "klingon")]
        // [InlineData("", "")]
        public async void ConvertParameterTitleAndLikelyParameterValuesReturnsUnknowns(string parameter,
            string likelyValue)
        {
            var typedTokens = await ConvertTokensToTypedTokens(parameter, likelyValue);

            Assert.Equal(2, typedTokens.Count());
            Assert.Equal(Tokens.ParameterTitle, typedTokens.ElementAt(0).Item2);
            Assert.Equal(Tokens.Unknown, typedTokens.ElementAt(1).Item2);
        }
    }
}