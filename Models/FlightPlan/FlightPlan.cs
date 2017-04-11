using System.Runtime.Serialization;

using Interfaces.FlightPlan;

namespace Models.FlightPlan
{
    [DataContract]
    public class FlightPlan: IFlightPlan
    {
        [DataMember(Name = "activity")]
        public string Activity { get; set; }
        [DataMember(Name = "parameters")]
        public string[] Parameters { get; set; }
    }
}
