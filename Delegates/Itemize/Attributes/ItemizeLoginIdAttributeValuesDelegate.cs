using Models.Patterns;

namespace Delegates.Itemize.Attributes
{
    public class ItemizeLoginIdAttributeValuesDelegate : ItemizeAttributeValuesDelegate
    {
        public ItemizeLoginIdAttributeValuesDelegate() :
            base(AttributeValuesPatterns.LoginId)
        {
            // ...
        }
    }
}