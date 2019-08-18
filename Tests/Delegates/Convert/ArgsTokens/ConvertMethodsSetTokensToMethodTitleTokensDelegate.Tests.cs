using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;

using Interfaces.Delegates.Convert;

using Controllers.Collection;

using Models.ArgsTokens;

using TestModels.ArgsDefinitions;

namespace Delegates.Convert.ArgsTokens.Tests
{
    public class ConvertMethodsSetTokensToMethodTitleTokensDelegateTests
    {
        private IConvertDelegate<IEnumerable<(string Token, Tokens Type)>, IEnumerable<(string Token, Tokens Type)>> convertMethodsSetTokensToMethodTitleTokensDelegate;

        public ConvertMethodsSetTokensToMethodTitleTokensDelegateTests()
        {
            var collectionController = new CollectionController();

            this.convertMethodsSetTokensToMethodTitleTokensDelegate =
                new ConvertMethodsSetTokensToMethodTitleTokensDelegate(
                    ReferenceArgsDefinition.ArgsDefinition,
                    collectionController);
        }

        [Fact]
        public void CanConvertMethodsSetTokensToMethodTitleTokens()
        {
            var typedTokens = convertMethodsSetTokensToMethodTitleTokensDelegate.Convert(
                new (string, Tokens)[] { ("sync", Tokens.MethodsSet) });

            Assert.Equal(3, typedTokens.Count());
            foreach (var typedToken in typedTokens)
                Assert.Equal(Tokens.MethodTitle, typedToken.Type);
        }

        [Theory]
        [InlineData(Tokens.CollectionTitle)]
        [InlineData(Tokens.LikelyMethodsAbbrevation)]
        [InlineData(Tokens.LikelyParameterValue)]
        [InlineData(Tokens.MethodTitle)]
        [InlineData(Tokens.ParameterTitle)]
        [InlineData(Tokens.ParameterValue)]
        [InlineData(Tokens.Unknown)]
        public void ConvertMethodsSetTokensToMethodTitleTokensDelegatePassesThroughOtherTypes(Tokens tokenType)
        {
            var typedTokens = convertMethodsSetTokensToMethodTitleTokensDelegate.Convert(
                new (string, Tokens)[] { ("", tokenType) });

            Assert.Single(typedTokens);
            Assert.Equal(tokenType, typedTokens.ElementAt(0).Type);
        }
    }
}