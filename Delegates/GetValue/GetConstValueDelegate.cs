using Interfaces.Delegates.GetValue;

namespace Delegates.GetValue
{
    public abstract class GetConstValueDelegate<Type> : IGetValueDelegate<Type>
    {
        private readonly Type constValue;

        public GetConstValueDelegate(Type constValue)
        {
            this.constValue = constValue;
        }

        public Type GetValue()
        {
            return constValue;
        }
    }
}