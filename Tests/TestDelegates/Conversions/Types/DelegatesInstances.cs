using System;
using System.Reflection;
using Delegates.Conversions.Types;
using Interfaces.Delegates.Conversions;

namespace Tests.TestDelegates.Conversions.Types
{
    public static class DelegatesInstances
    {
        public static IConvertDelegate<Type, ConstructorInfo> TestConvertTypeToDependenciesConstructorInfoDelegate =
            new ConvertTypeToDependenciesConstructorInfoDelegate();

        public static IConvertDelegate<ConstructorInfo, Type[]> TestConvertConstructorInfoToDependenciesTypesDelegate =
            new ConvertConstructorInfoToDependenciesTypesDelegate(
                TestModels.Dependencies.TestContextReplacements.Map);

        public static IConvertDelegate<Type, object> TestConvertTypeToInstanceDelegate =
            new ConvertTypeToInstanceDelegate(
                TestConvertTypeToDependenciesConstructorInfoDelegate,
                TestConvertConstructorInfoToDependenciesTypesDelegate);
    }
}