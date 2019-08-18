using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;

using Interfaces.Delegates.Convert;

using Controllers.Collection;

using Delegates.Convert.ArgsTokens;
using Delegates.Confirm.ArgsTokens;

using Models.ArgsTokens;

using TestModels.ArgsDefinitions;

namespace Delegates.Convert.ArgsTokens.Tests
{
    public class ConvertTokensToLikelyTypedTokensDelegateTests
    {
        private IConvertDelegate<IEnumerable<string>, IEnumerable<(string, Tokens)>> convertTokensToLikelyTypedTokensDelegate;

        public ConvertTokensToLikelyTypedTokensDelegateTests()
        {
            var collectionController = new CollectionController();

            var confirmLikelyTokenTypeDelegate = new ConfirmLikelyTokenTypeDelegate(
                ReferenceArgsDefinition.ArgsDefinition,
                collectionController);

            this.convertTokensToLikelyTypedTokensDelegate = new ConvertTokensToLikelyTypedTokensDelegate(
                confirmLikelyTokenTypeDelegate);
        }

        [Theory]
        [InlineData("download", "productimages")]
        [InlineData("download", "accountproductimages")]
        [InlineData("download", "screenshots")]
        [InlineData("download", "productfiles")]
        [InlineData("prepare", "productimages")]
        [InlineData("prepare", "accountproductimages")]
        [InlineData("prepare", "screenshots")]
        [InlineData("prepare", "productfiles")]
        [InlineData("update", "products")]
        [InlineData("update", "gameproductdata")]
        [InlineData("update", "accountproducts")]
        [InlineData("update", "apiproducts")]
        [InlineData("update", "gamedetails")]
        [InlineData("update", "updated")]
        [InlineData("update", "wishlisted")]
        [InlineData("update", "screenshots")]
        public void CanConvertTokensToLikelyTypedTokensDelegateMethodTitlesCollectionTitles(params string[] tokens)
        {
            var likelyTypedTokens = convertTokensToLikelyTypedTokensDelegate.Convert(tokens);
            Assert.NotEmpty(likelyTypedTokens);
            Assert.Equal(2, likelyTypedTokens.Count());
            Assert.Equal(Tokens.MethodTitle, likelyTypedTokens.ElementAt(0).Item2);
            Assert.Equal(Tokens.CollectionTitle, likelyTypedTokens.ElementAt(1).Item2);
        }

        [Theory]
        [InlineData("authorize", "--username")]
        [InlineData("authorize", "--password")]
        [InlineData("download", "--id")]
        [InlineData("download", "--os")]
        [InlineData("download", "--lang")]
        [InlineData("prepare", "--id")]
        [InlineData("prepare", "--os")]
        [InlineData("prepare", "--lang")]
        [InlineData("update", "--id")]
        public void CanConvertTokensToLikelyTypedTokensDelegateMethodTitlesParameterTitles(params string[] tokens)
        {
            var likelyTypedTokens = convertTokensToLikelyTypedTokensDelegate.Convert(tokens);
            Assert.NotEmpty(likelyTypedTokens);
            Assert.Equal(2, likelyTypedTokens.Count());
            Assert.Equal(Tokens.MethodTitle, likelyTypedTokens.ElementAt(0).Item2);
            Assert.Equal(Tokens.ParameterTitle, likelyTypedTokens.ElementAt(1).Item2);
        }

        [Theory]
        [InlineData("productimages", "download")]
        [InlineData("productimages", "prepare")]
        [InlineData("products", "update")]
        public void CanConvertTokensToLikelyTypedTokensDelegateWrongOrderOfValidTokensProducesLikelyParameterValues(params string[] tokens)
        {
            var likelyTypedTokens = convertTokensToLikelyTypedTokensDelegate.Convert(tokens);
            Assert.NotEmpty(likelyTypedTokens);
            Assert.Equal(2, likelyTypedTokens.Count());
            Assert.Equal(Tokens.CollectionTitle, likelyTypedTokens.ElementAt(0).Item2);
            Assert.Equal(Tokens.LikelyParameterValue, likelyTypedTokens.ElementAt(1).Item2);
        }

        [Theory]
        [InlineData("")]
        [InlineData("arbitrarystring")]
        public void CanConvertTokensToLikelyTypedTokensDelegateRandomInputProducesLikelyParameterValues(params string[] tokens)
        {
            var likelyTypedTokens = convertTokensToLikelyTypedTokensDelegate.Convert(tokens);
            Assert.NotEmpty(likelyTypedTokens);
            Assert.Single(likelyTypedTokens);
            Assert.Equal(Tokens.LikelyParameterValue, likelyTypedTokens.ElementAt(0).Item2);
        }
    }
}