using System;

namespace Autofac.Configuration.Attribute
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class AutowiredAttribute : System.Attribute
    {
        public AutowiredAttribute()
        {

        }

        public AutowiredAttribute(string name)
        {
            Named = name;
        }
        public string Named { get; set; }

    }
}
