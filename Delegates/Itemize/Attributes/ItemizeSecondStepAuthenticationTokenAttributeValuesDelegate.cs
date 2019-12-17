using Models.Patterns;

namespace Delegates.Itemize.Attributes
{
    public class ItemizeSecondStepAuthenticationTokenAttributeValuesDelegate : ItemizeAttributeValuesDelegate
    {
        public ItemizeSecondStepAuthenticationTokenAttributeValuesDelegate() :
            base(AttributeValuesPatterns.SecondStepAuthenticationToken)
        {
            // ...
        }
    }
}