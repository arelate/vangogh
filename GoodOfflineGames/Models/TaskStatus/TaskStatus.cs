using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Interfaces.TaskStatus;

namespace Models.TaskStatus
{
    [DataContract]
    [KnownType(typeof(TaskStatus))]
    [KnownType(typeof(TaskProgress))]
    public class TaskStatus: ITaskStatus
    {
        [DataMember(Name = "complete")]
        public bool Complete { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "progress")]
        public ITaskProgress Progress { get; set; }
        [DataMember(Name = "started")]
        public DateTime Started { get; set; } = DateTime.UtcNow;
        [DataMember(Name = "completed")]
        public DateTime Completed { get; set; }
        [DataMember(Name = "children")]
        public IList<ITaskStatus> Children { get; set; }
        [DataMember(Name = "warnings")]
        public IList<string> Warnings { get; set; }
        [DataMember(Name = "failures")]
        public IList<string> Failures { get; set; }
        [DataMember(Name = "information")]
        public IList<string> Information { get; set; }
    }

    [DataContract]
    public class TaskProgress: ITaskProgress
    {
        [DataMember(Name = "target")]
        public string Target { get; set; }
        [DataMember(Name = "current")]
        public long Current { get; set; }
        [DataMember(Name = "total")]
        public long Total { get; set; }
        [DataMember(Name = "unit")]
        public string Unit { get; set; }
    }
}
