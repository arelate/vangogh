using Interfaces.Delegates.Values;

namespace Delegates.Values
{
    public abstract class GetObjectValueDelegate<T> : IGetValueDelegate<T, string>
        where T : class, new()
    {
        private readonly T value;

        public GetObjectValueDelegate()
        {
            value = new T();
        }

        public T GetValue(string input)
        {
            return value;
        }
    }
}