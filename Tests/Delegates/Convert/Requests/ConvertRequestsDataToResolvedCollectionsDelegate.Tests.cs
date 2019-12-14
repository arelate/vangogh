// using System;
// using System.Collections.Generic;

// using Xunit;

// using Controllers.Collection;

// using Interfaces.Delegates.Convert;

// using Delegates.Convert.Requests;

// using Models.Requests;

// using TestModels.ArgsDefinitions;

// namespace Delegates.Convert.Requests.Tests
// {
//     public class ConvertRequestsDataToResolvedCollectionsDelegateTests
//     {
//         private IConvertDelegate<RequestsData, RequestsData> convertRequestsDataToResolvedCollectionsDelegate;

//         public ConvertRequestsDataToResolvedCollectionsDelegateTests()
//         {
//             // var collectionController = new CollectionController();

//             convertRequestsDataToResolvedCollectionsDelegate = singletonInstancesController.GetInstance(
//                 typeof(ConvertRequestsDataToResolvedCollectionsDelegate))
//                 as ConvertRequestsDataToResolvedCollectionsDelegate;
                
//                 // new ConvertRequestsDataToResolvedCollectionsDelegate(
//                 //     ReferenceArgsDefinition.ArgsDefinition,
//                 //     collectionController);
//         }

//         [Theory]
//         [InlineData(8, "update")] // no collections - should use all as default
//         [InlineData(1, "update", "products")] // applicable collection
//         [InlineData(9, "update", "productfiles")] // not applicable collection
//         [InlineData(0, "authorize")] // no collection and none expected
//         [InlineData(1, "authorize", "products")] // not applicable collection
//         public void CanConvertRequestsDataToResolvedCollections(
//             int expectedCollectionsCount,
//             string method, 
//             params string[] collections)
//         {
//             var requestsData = new RequestsData();
//             requestsData.Methods.Add(method);
//             requestsData.Collections.AddRange(collections);

//             var requestsDataWithCollections =
//                 convertRequestsDataToResolvedCollectionsDelegate.Convert(
//                     requestsData);

//             Assert.Equal(expectedCollectionsCount, requestsDataWithCollections.Collections.Count);
//         }
//     }
// }