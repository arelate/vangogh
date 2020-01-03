using System;
using System.Collections.Generic;

using Xunit;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Instances;

using Attributes;

using Delegates.Itemize.Types;
using Delegates.Itemize.Types.Attributes;

namespace Controllers.Instances.Tests
{
    public class SingletonInstancesControllerTests
    {
        private static IInstancesController dependenciesInstancesController;
        private static IInstancesController testDependenciesOverridesInstancesController;

        public SingletonInstancesControllerTests()
        {
            dependenciesInstancesController = new SingletonInstancesController();
            testDependenciesOverridesInstancesController = new SingletonInstancesController(true);
        }

        private static IEnumerable<object[]> EnumerateTypesWithConstructorAttribute(Type attributeType)
        {
            var itemizeAllTypesDelegate = new ItemizeAllTypesDelegate();
            var itemizeAllDependenciesAttributeTypesDelegate = new ItemizeAllDependenciesAttributeTypesDelegate(itemizeAllTypesDelegate);

            foreach (var type in itemizeAllDependenciesAttributeTypesDelegate.ItemizeAll())
                yield return new object[] { type };
        }

        public static IEnumerable<object[]> EnumerateTypesWithDependencies()
        {
            return EnumerateTypesWithConstructorAttribute(typeof(DependenciesAttribute));
        }

        public static IEnumerable<object[]> EnumerateTypesWithTestDependenciesOverrides()
        {
            return EnumerateTypesWithConstructorAttribute(typeof(TestDependenciesOverridesAttribute));
        }

        private void CanInstantiateTypes(
            IInstancesController instanceController,
            params Type[] types)
        {
            Assert.NotEmpty(types);
            Assert.NotNull(types);

            var instances = dependenciesInstancesController.GetInstances(types);
            Assert.NotNull(instances);

            foreach (var instance in instances)
                Assert.NotNull(instance);
        }

        private void ConstructorParametersAndTypeMatchDependencies(
            IInstancesController instancesController,
            params Type[] types)
        {
            Assert.NotNull(types);
            Assert.NotEmpty(types);

            foreach (var type in types)
            {
                var constructorInfo = instancesController.GetInstantiationConstructorInfo(type);
                var parameters = constructorInfo.GetParameters();
                var dependenciesTypes =
                    instancesController.GetTypesForConstructor(
                        constructorInfo);
                Assert.Equal(dependenciesTypes.Length, parameters.Length);
                for (var ii = 0; ii < parameters.Length; ii++)
                {
                    var instance = instancesController.GetInstance(dependenciesTypes[ii]);
                    Assert.IsAssignableFrom(parameters[ii].ParameterType, instance);
                }
            }
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithDependencies))]
        public void InstancesControllerCanInitializeAllDeclaredDependencies(params Type[] types)
        {
            CanInstantiateTypes(dependenciesInstancesController, types);
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithTestDependenciesOverrides))]
        public void InstancesControllerCanInitializeAllDeclaredTestDependenciesOverrides(params Type[] types)
        {
            CanInstantiateTypes(testDependenciesOverridesInstancesController, types);
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithTestDependenciesOverrides))]
        public void TestDependenciesOverridesMatchesDependenciesCount(params Type[] types)
        {
            foreach (var type in types)
            {
                var constructorInfo = dependenciesInstancesController.GetInstantiationConstructorInfo(type);
                Assert.NotNull(constructorInfo);

                var dependenciesTypes =
                    dependenciesInstancesController.GetTypesForConstructor(
                        constructorInfo);
                Assert.NotNull(dependenciesTypes);
                Assert.NotEmpty(dependenciesTypes);

                var testOverridesDependenciesTypes =
                    testDependenciesOverridesInstancesController.GetTypesForConstructor(
                        constructorInfo);
                Assert.NotNull(testOverridesDependenciesTypes);
                Assert.NotEmpty(testOverridesDependenciesTypes);

                Assert.Equal(dependenciesTypes.Length, testOverridesDependenciesTypes.Length);
            }
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithDependencies))]
        public void NumberOfDependenciesMatchesNumberOfConstructorParameters(params Type[] types)
        {
            ConstructorParametersAndTypeMatchDependencies(dependenciesInstancesController, types);
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithTestDependenciesOverrides))]
        public void NumberOfTestDependenciesOverridesMatchesNumberOfConstructorParameters(params Type[] types)
        {
            ConstructorParametersAndTypeMatchDependencies(testDependenciesOverridesInstancesController, types);
        }

        [Fact]
        public void InstantiationConstructorInfoIsNullWhenDependencyAttributesUndefined()
        {
            Assert.Null(dependenciesInstancesController.GetInstantiationConstructorInfo(typeof(string)));
        }
    }
}