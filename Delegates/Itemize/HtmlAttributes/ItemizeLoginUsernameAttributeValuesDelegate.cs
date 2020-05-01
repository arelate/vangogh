using Models.Patterns;

namespace Delegates.Itemize.HtmlAttributes
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