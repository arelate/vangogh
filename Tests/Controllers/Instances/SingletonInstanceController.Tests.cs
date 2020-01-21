using System;
using System.Collections.Generic;

using Xunit;

using Interfaces.Delegates.Itemize;

using Interfaces.Controllers.Instances;
using Interfaces.Models.Dependencies;

using Attributes;

using Delegates.Itemize.Types;
using Delegates.Itemize.Types.Attributes;

namespace Controllers.Instances.Tests
{
    public class SingletonInstancesControllerTests
    {
        private static IInstancesController dependenciesInstancesController;
        private static IInstancesController testDependenciesInstancesController;
        private static IItemizeAllDelegate<Type> itemizeAllTypesDelegate = new ItemizeAllTypesDelegate();
        private static IItemizeAllDelegate<Type> itemizeAllDependenciesAttributeTypesDelegate =
            new ItemizeAllDependenciesAttributeTypesDelegate(itemizeAllTypesDelegate);

        public SingletonInstancesControllerTests()
        {
            dependenciesInstancesController = new SingletonInstancesController();
            testDependenciesInstancesController = new SingletonInstancesController(DependencyContext.Default | DependencyContext.Test);
        }

        private static IEnumerable<object[]> EnumerateTypesWithConstructorAttribute(IItemizeAllDelegate<Type> itemizeAllAttributeTypesDelegate)
        {
            foreach (var type in itemizeAllAttributeTypesDelegate.ItemizeAll())
                yield return new object[] { type };
        }
        public static IEnumerable<object[]> EnumerateTypesWithDependencies()
        {
            return EnumerateTypesWithConstructorAttribute(itemizeAllDependenciesAttributeTypesDelegate);
        }

        public static IEnumerable<object[]> EnumerateTypesWithTestDependencies()
        {
            foreach (var type in itemizeAllDependenciesAttributeTypesDelegate.ItemizeAll())
            {
                foreach (var constructor in type.GetConstructors())
                {
                    var dependenciesAttributes = constructor.GetCustomAttributes(typeof(DependenciesAttribute), true);

                    var hasTestDependencies = false;
                    foreach (var dependenciesAttribute in dependenciesAttributes)
                    {
                        if ((dependenciesAttribute as DependenciesAttribute).Context == DependencyContext.Test)
                            hasTestDependencies = true;
                    }
                    if (!hasTestDependencies) continue;
                    yield return new object[] { type };
                }
            }
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
        [MemberData(nameof(EnumerateTypesWithTestDependencies))]
        public void InstancesControllerCanInitializeAllDeclaredTestDependenciesOverrides(params Type[] types)
        {
            CanInstantiateTypes(testDependenciesInstancesController, types);
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithTestDependencies))]
        public void TestDependenciesDifferFromDefaultDependencies(params Type[] types)
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

                var testDependenciesTypes =
                    testDependenciesInstancesController.GetTypesForConstructor(
                        constructorInfo);
                Assert.NotNull(testDependenciesTypes);
                Assert.NotEmpty(testDependenciesTypes);

                Assert.Equal(dependenciesTypes.Length, testDependenciesTypes.Length);

                var allDependenciesAreEqual = true;
                for (var ii = 0; ii < dependenciesTypes.Length; ii++)
                    allDependenciesAreEqual &= dependenciesTypes[ii] == testDependenciesTypes[ii];

                Assert.False(allDependenciesAreEqual);
            }
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithTestDependencies))]
        public void TestDependenciesHaveDefaultDependenciesSpecified(params Type[] types)
        {
            foreach (var type in types)
            {
                var constructorInfo = dependenciesInstancesController.GetInstantiationConstructorInfo(type);
                Assert.NotNull(constructorInfo);

                var dependenciesAttributes = constructorInfo.GetCustomAttributes(typeof(DependenciesAttribute), true);

                Assert.True(dependenciesAttributes.Length > 1);

                var hasDefaultDependency = false;
                foreach (var dependenciesAttribute in dependenciesAttributes)
                {
                    hasDefaultDependency |= (dependenciesAttribute as DependenciesAttribute).Context == DependencyContext.Default;
                }

                Assert.True(hasDefaultDependency);
            }
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithTestDependencies))]
        public void TestDependenciesHaveSameNumberOfDependenciesAsDefault(params Type[] types)
        {
            foreach (var type in types)
            {
                var constructorInfo = dependenciesInstancesController.GetInstantiationConstructorInfo(type);
                Assert.NotNull(constructorInfo);

                var dependenciesAttributes = constructorInfo.GetCustomAttributes(typeof(DependenciesAttribute), true);

                Assert.True(dependenciesAttributes.Length > 1);

                DependenciesAttribute defaultDependencies = null;
                DependenciesAttribute testDependencies = null;

                foreach (var dependenciesAttribute in dependenciesAttributes)
                {
                    var dependencies = dependenciesAttribute as DependenciesAttribute;
                    switch (dependencies.Context)
                    {
                        case DependencyContext.Default:
                            defaultDependencies = dependencies;
                            break;
                        case DependencyContext.Test:
                            testDependencies = dependencies;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }

                Assert.NotNull(defaultDependencies);
                Assert.NotNull(testDependencies);

                Assert.NotNull(defaultDependencies.Dependencies);
                Assert.NotNull(testDependencies.Dependencies);

                Assert.Equal(defaultDependencies.Dependencies.Length, testDependencies.Dependencies.Length);
            }
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithDependencies))]
        public void NumberOfDependenciesMatchesNumberOfConstructorParameters(params Type[] types)
        {
            ConstructorParametersAndTypeMatchDependencies(dependenciesInstancesController, types);
        }

        [Theory]
        [MemberData(nameof(EnumerateTypesWithTestDependencies))]
        public void NumberOfTestDependenciesOverridesMatchesNumberOfConstructorParameters(params Type[] types)
        {
            ConstructorParametersAndTypeMatchDependencies(testDependenciesInstancesController, types);
        }

        [Fact]
        public void InstantiationConstructorInfoIsNullWhenDependencyAttributesUndefined()
        {
            Assert.Null(dependenciesInstancesController.GetInstantiationConstructorInfo(typeof(string)));
        }
    }
}