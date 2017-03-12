using Interfaces.Naming;

using Models.Separators;

namespace Controllers.Naming
{
    public class ActivityParametersNameDelegate : IGetNameDelegate
    {
        public string GetName(params string[] nameParts)
        {
            return string.Join(Separators.ActivityParameters, nameParts);
        }
    }
}
