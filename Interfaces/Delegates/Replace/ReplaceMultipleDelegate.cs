namespace Interfaces.Delegates.Replace
{
    public interface IReplaceMultipleDelegate<Type>
    {
        Type ReplaceMultiple(Type data, Type replaceWith, params Type[] findWhat);
    }
}