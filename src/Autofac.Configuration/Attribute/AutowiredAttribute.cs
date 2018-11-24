using System;

namespace Autofac.Configuration.Attribute
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class AutowiredAttribute : System.Attribute
    {


    }
}
