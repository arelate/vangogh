using System;
using System.Reflection;
using Attributes;
using Interfaces.Delegates.Conversions;

namespace Delegates.Conversions.Types
{
    public sealed class ConvertTypeToDependenciesConstructorInfoDelegate : IConvertDelegate<Type, ConstructorInfo>
    {
        public ConstructorInfo Convert(Type type)
        {
            foreach (var constructorInfo in type.GetConstructors())
                if (constructorInfo.IsDefined(typeof(DependenciesAttribute)))
                    return constructorInfo;

            return null;
        }
    }
}