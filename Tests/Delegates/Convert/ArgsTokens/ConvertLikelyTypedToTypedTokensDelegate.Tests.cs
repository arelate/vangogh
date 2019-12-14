// using System;
// using System.Linq;
// using System.Collections.Generic;

// using Xunit;

// using Interfaces.Delegates.Convert;

// // using Controllers.Collection;
// using Controllers.Instances;

// using Models.ArgsTokens;

// using TestModels.ArgsDefinitions;

// namespace Delegates.Convert.ArgsTokens.Tests
// {
//     public class ConvertLikelyTypedToTypedTokensDelegateTests
//     {
//         private IConvertDelegate<IEnumerable<(string Token, Tokens Type)>, IEnumerable<(string Token, Tokens Type)>> convertLikelyTypedToTypedTokensDelegate;

//         public ConvertLikelyTypedToTypedTokensDelegateTests()
//         {
//             // var collectionController = new CollectionController();
//             var singletonInstancesController = new SingletonInstancesController();

//             this.convertLikelyTypedToTypedTokensDelegate = singletonInstancesController.GetInstance(
//                 typeof(ConvertLikelyTypedToTypedTokensDelegate))
//                 as ConvertLikelyTypedToTypedTokensDelegate;
            
//                 // new ConvertLikelyTypedToTypedTokensDelegate(
//                 //     ReferenceArgsDefinition.ArgsDefinition,
//                 //     collectionController);
//         }

//         [Theory]
//         [InlineData("-upd")]
//         [InlineData("-updx")] // ends with unknown token
//         [InlineData("-xupd")] // starts with unknown token
//         public void ConvertLikelyMethodAbbrevationToMethodTitleTokens(string token)
//         {
//             var typedTokens = convertLikelyTypedToTypedTokensDelegate.Convert(
//                 new (string, Tokens)[] { (token, Tokens.LikelyMethodsAbbrevation) });

//             var methodTitlesCount = 0;
//             foreach (var typedToken in typedTokens)
//             {
//                 if (typedToken.Type == Tokens.Unknown) continue;
//                 Assert.Equal(Tokens.MethodTitle, typedToken.Type);
//                 methodTitlesCount++;
//             }

//             Assert.Equal(3, methodTitlesCount);
//         }

//         [Theory]
//         [InlineData("-")]
//         [InlineData(null)]
//         [InlineData("")]
//         public void ConvertLikelyMethodAbbrevationHandlesInvalidInputs(string token)
//         {
//             var typedTokens = convertLikelyTypedToTypedTokensDelegate.Convert(
//                 new (string, Tokens)[] { (token, Tokens.LikelyMethodsAbbrevation) }
//             );

//             Assert.NotNull(typedTokens);
//             Assert.Empty(typedTokens);
//         }

//         [Fact]
//         public void ConvertLikelyTypedToTypedTokenDelegateHandlesEmptyInput()
//         {
//             var typedTokens = convertLikelyTypedToTypedTokensDelegate.Convert(
//                 new (string, Tokens)[0]);

//             Assert.NotNull(typedTokens);
//             Assert.Empty(typedTokens);
//         }

//         [Fact]
//         public void ConvertLikelyTypedToTypedTokenDelegateHandlesNullInput()
//         {
//             Assert.Throws<ArgumentNullException>(
//                 () =>
//                 {
//                     var typedTokens = convertLikelyTypedToTypedTokensDelegate.Convert(null);
//                     Assert.Empty(typedTokens);
//                 });
//         }

//         [Theory]
//         [InlineData("--id")]
//         [InlineData("--arbitrarystring")] // starts with two dashes
//         public void ConvertParameterTitleToParameterTitleTrimLeadingDash(string token)
//         {
//             var typedTokens = convertLikelyTypedToTypedTokensDelegate.Convert(
//                 new (string, Tokens)[] { (token, Tokens.ParameterTitle) });

//             foreach (var typedToken in typedTokens)
//                 Assert.Equal(Tokens.ParameterTitle, typedToken.Type);
//         }

//         [Theory]
//         [InlineData("--os", "windows")]
//         [InlineData("--os", "osx")]
//         [InlineData("--os", "linux")]
//         [InlineData("--lang", "en")]
//         [InlineData("--id", "arbitrarystring")] // parameter that doesn't have values
//         public void ConvertParameterTitleAndLikelyParameterValuesSucceeds(string parameter, string likelyValue)
//         {
//             var typedTokens = convertLikelyTypedToTypedTokensDelegate.Convert(
//                 new (string, Tokens)[] {
//                     (parameter, Tokens.ParameterTitle),
//                     (likelyValue, Tokens.LikelyParameterValue) });

//             Assert.Equal(2, typedTokens.Count());
//             Assert.Equal(Tokens.ParameterTitle, typedTokens.ElementAt(0).Type);
//             Assert.Equal(Tokens.ParameterValue, typedTokens.ElementAt(1).Type);
//         }

//         [Theory]
//         [InlineData("--os", "bsd")]
//         [InlineData("--lang", "klingon")]
//         [InlineData("", "")]
//         public void ConvertParameterTitleAndLikelyParameterValuesReturnsUnknowns(string parameter, string likelyValue)
//         {
//             var typedTokens = convertLikelyTypedToTypedTokensDelegate.Convert(
//                 new (string, Tokens)[] {
//                     (parameter, Tokens.ParameterTitle),
//                     (likelyValue, Tokens.LikelyParameterValue) });    

//             Assert.Equal(2, typedTokens.Count());
//             Assert.Equal(Tokens.ParameterTitle, typedTokens.ElementAt(0).Type);
//             Assert.Equal(Tokens.Unknown, typedTokens.ElementAt(1).Type);        
//         }
//     }
// }