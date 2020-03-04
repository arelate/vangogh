
using System;

namespace Models.ArgsTokens
{
    [Flags]
    public enum Tokens
    {
        Unknown = 0,
        LikelyMethodsAbbrevation,
        MethodsSet,
        MethodTitle,
        CollectionTitle,
        ParameterTitle,
        LikelyParameterValue,
        ParameterValue    
    }
}
