using Models.Patterns;

namespace Delegates.Itemizations.HtmlAttributes
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