namespace Interfaces.Eligibility
{
    public interface IEligibilityDelegate<Type>
    {
        bool IsEligible(Type item);
    }
}
