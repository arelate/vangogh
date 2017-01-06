using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Interfaces.TaskStatus;

namespace Models.TaskStatus
{
    [DataContract]
    public class TaskStatus: ITaskStatus
    {
        [DataMember(Name = "complete")]
        public bool Complete { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "progress")]
        public ITaskProgress Progress { get; set; }
        [DataMember(Name = "started")]
        public DateTime Started { get; set; }
        [DataMember(Name = "completed")]
        public DateTime Completed { get; set; }
        [DataMember(Name = "childTasks")]
        public IList<ITaskStatus> ChildTasks { get; set; }
    }

    [DataContract]
    public class TaskProgress: ITaskProgress
    {
        [DataMember(Name = "current")]
        public long Current { get; set; }
        [DataMember(Name = "total")]
        public long Total { get; set; }
        [DataMember(Name = "unit")]
        public string Unit { get; set; }
    }
}
