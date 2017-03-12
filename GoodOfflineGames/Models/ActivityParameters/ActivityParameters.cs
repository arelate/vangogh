using System.Runtime.Serialization;

using Interfaces.ActivityParameters;

namespace Models.ActivityParameters
{
    [DataContract]
    public class ActivityParameters: IActivityParameters
    {
        [DataMember(Name = "activity")]
        public string Activity { get; set; }
        [DataMember(Name = "parameters")]
        public string[] Parameters { get; set; }
    }
}
