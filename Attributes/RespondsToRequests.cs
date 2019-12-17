using System;

namespace Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RespondsToRequests : Attribute
    {
        public string Method { get; set; }
        public string Collection { get; set; }
    }
}