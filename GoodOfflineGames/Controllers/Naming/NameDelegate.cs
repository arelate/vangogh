using Interfaces.Naming;

using Models.Separators;

namespace Controllers.Naming
{
    public class NameDelegate : IGetNameDelegate
    {
        public string GetName(params string[] nameParts)
        {
            return string.Join(Separators.FlightPlan, nameParts);
        }
    }
}
