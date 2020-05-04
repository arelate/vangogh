using System;
using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Itemizations;

namespace Delegates.Itemizations.Types.Attributes
{
    public class ItemizeDependenciesAttributesForTypeDelegate : IItemizeDelegate<Type, DependenciesAttribute>
    {
        public IEnumerable<DependenciesAttribute> Itemize(Type type)
        {
            foreach (var constructor in type.GetConstructors())
            {
                var dependenciesAttributes = constructor.GetCustomAttributes(typeof(DependenciesAttribute), true);
                foreach (var dependenciesAttribute in dependenciesAttributes)
                    yield return dependenciesAttribute as DependenciesAttribute;
            }
        }
    }
}