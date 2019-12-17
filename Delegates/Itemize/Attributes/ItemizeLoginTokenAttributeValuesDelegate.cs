using Models.Patterns;

namespace Delegates.Itemize.Attributes
{
    public class ItemizeLoginTokenAttributeValuesDelegate : ItemizeAttributeValuesDelegate
    {
        public ItemizeLoginTokenAttributeValuesDelegate() :
            base(AttributeValuesPatterns.LoginToken)
        {
            // ...
        }
    }
}