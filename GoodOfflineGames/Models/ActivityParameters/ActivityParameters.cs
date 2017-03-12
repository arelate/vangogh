using System.Runtime.Serialization;

namespace Models.ActivityParameters
{
    [DataContract]
    public class ActivityParameters
    {
        [DataMember(Name = "activity")]
        public string Activity { get; set; }
        [DataMember(Name = "parameters")]
        public string[] Parameters { get; set; }
    }
}
