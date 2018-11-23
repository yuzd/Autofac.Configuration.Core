using System;

namespace Autofac.Configuration.Attribute
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class BeanAttribute : System.Attribute
    {

        /// <summary>
        /// 别名
        /// </summary>
        public string Named { get; set; }

        /// <summary>
        /// 作用域
        /// </summary>
        public AutofucScope AutofucScope { get; set; } = AutofucScope.InstancePerLifetimeScope;

    }

}
