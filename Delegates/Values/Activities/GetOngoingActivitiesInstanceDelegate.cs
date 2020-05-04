using System.Collections.Generic;
using Interfaces.Models.Activities;

namespace Delegates.Values.Activities
{
    public class GetOngoingActivitiesInstanceDelegate : GetSingletonInstanceDelegate<Stack<IActivity>>
    {
        // ...
    }
}