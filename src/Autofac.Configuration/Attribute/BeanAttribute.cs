using System;

namespace Autofac.Configuration.Attribute
{

    /// <summary>
    /// 自动识别并注册为autofac的容器中
    /// 支持放在类 和 方法上面
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BeanAttribute : System.Attribute
    {
        public BeanAttribute()
        {

        }

        public BeanAttribute(string name)
        {
            this.Named = name;
        }

        public BeanAttribute(Type asType)
        {
            this.As = asType;
        }
        /// <summary>
        /// 指定注册类型
        /// </summary>
        public Type As { get; set; }

        /// <summary>
        /// 拦截器类型
        /// </summary>
        public Type InterceptorType{ get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string Named { get; set; }

        /// <summary>
        /// 作用域
        /// </summary>
        public AutofacScope AutofacScope { get; set; } = AutofacScope.InstancePerLifetimeScope;

    }

}
