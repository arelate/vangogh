// using System;
// using System.Linq;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// using Xunit;

// using Interfaces.Delegates.Convert;

// using Controllers.Collection;
// using Controllers.Instances;

// using Models.ArgsTokens;

// using TestModels.ArgsDefinitions;

// namespace Delegates.Convert.ArgsTokens.Tests
// {
//     public class ConvertMethodsSetTokensToMethodTitleTokensDelegateTests
//     {
//         private IConvertAsyncDelegate<IAsyncEnumerable<(string Token, Tokens Type)>, IAsyncEnumerable<(string Token, Tokens Type)>> convertMethodsSetTokensToMethodTitleTokensDelegate;
//         private Models.Status.Status testStatus;

//         public ConvertMethodsSetTokensToMethodTitleTokensDelegateTests()
//         {
//             // var collectionController = new CollectionController();
//             var singletonInstancesController = new SingletonInstancesController();

//             this.convertMethodsSetTokensToMethodTitleTokensDelegate = singletonInstancesController.GetInstance(
//                 typeof(ConvertMethodsSetTokensToMethodTitleTokensDelegate))
//                 as ConvertMethodsSetTokensToMethodTitleTokensDelegate;

//             var testStatus = new Models.Status.Status();

//             // new ConvertMethodsSetTokensToMethodTitleTokensDelegate(
//             //     ReferenceArgsDefinition.ArgsDefinition,
//             //     collectionController);
//         }

//         [Fact]
//         public async Task CanConvertMethodsSetTokensToMethodTitleTokens()
//         {
//             var typedTokens = new List<(string, Tokens)>();
//             await foreach (var typedToken in
//             convertMethodsSetTokensToMethodTitleTokensDelegate.ConvertAsync(
//                 new (string, Tokens)[] { ("sync", Tokens.MethodsSet) },
//                 testStatus))
//             {
//                 typedTokens.Add(typedToken);
//             }

//             Assert.Equal(3, typedTokens.Count());
//             foreach (var typedToken in typedTokens)
//                 Assert.Equal(Tokens.MethodTitle, typedToken.Type);
//         }

//         [Theory]
//         [InlineData(Tokens.CollectionTitle)]
//         [InlineData(Tokens.LikelyMethodsAbbrevation)]
//         [InlineData(Tokens.LikelyParameterValue)]
//         [InlineData(Tokens.MethodTitle)]
//         [InlineData(Tokens.ParameterTitle)]
//         [InlineData(Tokens.ParameterValue)]
//         [InlineData(Tokens.Unknown)]
//         public void ConvertMethodsSetTokensToMethodTitleTokensDelegatePassesThroughOtherTypes(Tokens tokenType)
//         {
//             var typedTokens = convertMethodsSetTokensToMethodTitleTokensDelegate.Convert(
//                 new (string, Tokens)[] { ("", tokenType) });

//             Assert.Single(typedTokens);
//             Assert.Equal(tokenType, typedTokens.ElementAt(0).Type);
//         }
//     }
// }