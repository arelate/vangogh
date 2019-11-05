using Models.Patterns;

namespace Delegates.Itemize.Attributes
{
    public class ItemizeLoginUsernameAttributeValuesDelegate : ItemizeAttributeValuesDelegate
    {
        public ItemizeLoginUsernameAttributeValuesDelegate() :
            base(AttributeValuesPatterns.LoginUsername)
        {
            // ...
        }
    }
}