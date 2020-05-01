namespace Interfaces.Delegates.Replace
{
    // TODO: Convert?
    public interface IReplaceMultipleDelegate<Type>
    {
        Type ReplaceMultiple(Type data, Type replaceWith, params Type[] findWhat);
    }
}