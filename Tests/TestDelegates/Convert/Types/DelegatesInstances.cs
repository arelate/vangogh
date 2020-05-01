using System;
using System.Reflection;
using Interfaces.Delegates.Convert;
using Delegates.Convert.Types;

namespace Tests.TestDelegates.Convert.Types
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