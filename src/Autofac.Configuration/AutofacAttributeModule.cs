using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac.Builder;
using Autofac.Configuration.Attribute;
using Autofac.Core;
using Autofac.Extras.DynamicProxy;
using Castle.Components.DictionaryAdapter;
using Castle.DynamicProxy.Internal;

namespace Autofac.Configuration
{
    /// <summary>
    /// Module for configuration parsing and registration.
    /// </summary>
    public class AutofacAttributeModule : Module
    {
        private readonly Assembly[] _assemblyList;

        public AutofacAttributeModule(params Assembly[] assemblyList)
        {
            if (assemblyList.Length < 1)
            {
                throw new ArgumentException(nameof(assemblyList));
            }

            _assemblyList = assemblyList;
        }

        public AutofacAttributeModule(params string[] assemblyNameList)
        {
            if (assemblyNameList.Length < 1)
            {
                throw new ArgumentException(nameof(assemblyNameList));
            }

            var all =GetAssemblies();
            _assemblyList = all.Where(assembly => assemblyNameList.Contains(assembly.GetName().Name)).ToArray();
        }

        private IEnumerable<Assembly> GetAssemblies()
        {
            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());

            do
            {
                var asm = stack.Pop();

                yield return asm;

                foreach (var reference in asm.GetReferencedAssemblies())
                    if (!list.Contains(reference.FullName))
                    {
                        stack.Push(Assembly.Load(reference));
                        list.Add(reference.FullName);
                    }

            }
            while (stack.Count > 0);

        }
        protected override void Load(ContainerBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (_assemblyList == null || _assemblyList.Length<1)
            {
                throw new ArgumentNullException(nameof(_assemblyList));
            }
            var assemblyList = _assemblyList.Distinct();
            foreach (var assembly in assemblyList)
            {
                var types = assembly.GetExportedTypes();
                //找到类型中含有 Bean 标签的类
                var beanTypeList = (from type in types
                    let bean = type.GetCustomAttribute<BeanAttribute>()
                    where type.IsClass && !type.IsAbstract && bean != null
                    select new
                    {
                        Type = type,
                        Bean = bean
                    }).ToList();

                foreach (var beanType in beanTypeList)
                {
                    //有指定类型
                    if (beanType.Bean.As != null)
                    {
                        if (beanType.Bean.As.IsAssignableFrom(beanType.Type))
                        {
                            builder.BindBuildWithScope(beanType.Type, beanType.Bean);
                            continue;
                        }

                        //指定的类型并不是它的基类

                    }

                    //注册为自己的类型
                    builder.BindBuildWithScope(beanType.Type,beanType.Bean);

                    if (beanType.Type.BaseType != null && beanType.Type.BaseType != typeof(object))
                    {
                        //注册为自己的父类
                        beanType.Bean.As = beanType.Type.BaseType;
                        builder.BindBuildWithScope(beanType.Type,beanType.Bean);
                    }

                    var interfaces = beanType.Type.GetAllInterfaces();
                    if (interfaces.Length == 1)
                    {
                        //如果只实现了一个接口那么就认为他是以该接口为对应注册
                        beanType.Bean.As = interfaces[0];
                        builder.BindBuildWithScope(beanType.Type,beanType.Bean);
                    }

                }
            }
        }
    }

    public static class AutofucAttributeExtentions
    {
        /// <summary>
        /// 注册到autofac
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="fromType"></param>
        /// <param name="bean"></param>
        public static void BindBuildWithScope(this ContainerBuilder builder,Type fromType, BeanAttribute bean )
        {
            AutofacScope scope = bean.AutofacScope;
            Type toType = bean.As;
            string name = bean.Named;
            var registrar = builder.RegisterType(fromType);
            switch (scope)
            {
                case AutofacScope.SingleInstance:
                    if (toType == null)
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            registrar.Named(name,fromType).SingleInstance().PropertiesByAttributeAutowired();
                        }
                        else
                        {
                            registrar.SingleInstance().PropertiesByAttributeAutowired();
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            registrar.As(toType).Named(name,toType).SingleInstance().PropertiesByAttributeAutowired();
                        }
                        else
                        {
                            registrar.As(toType).SingleInstance().PropertiesByAttributeAutowired();
                        }

                    }

                    break;
                case AutofacScope.InstancePerDependency:
                    if (toType == null)
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            registrar.Named(name,fromType).InstancePerDependency().PropertiesByAttributeAutowired();
                        }
                        else
                        {
                            registrar.InstancePerDependency().PropertiesByAttributeAutowired();
                        }

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            registrar.As(toType).Named(name,toType).InstancePerDependency().PropertiesByAttributeAutowired();
                        }
                        else
                        {
                            registrar.As(toType).InstancePerDependency().PropertiesByAttributeAutowired();
                        }
                    }

                    break;
                case AutofacScope.InstancePerOwned:
                    if (toType == null)
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            registrar.Named(name,fromType).InstancePerLifetimeScope().PropertiesByAttributeAutowired();
                        }
                        else
                        {
                            registrar.InstancePerLifetimeScope().PropertiesByAttributeAutowired();
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            registrar.As(toType).Named(name,toType).InstancePerOwned(toType).PropertiesByAttributeAutowired();
                        }
                        else
                        {
                            registrar.As(toType).InstancePerOwned(toType).PropertiesByAttributeAutowired();
                        }

                    }

                    break;
                case AutofacScope.InstancePerRequest:
                    if (toType == null)
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            registrar.Named(name,fromType).InstancePerRequest().PropertiesByAttributeAutowired();
                        }
                        else
                        {
                            registrar.InstancePerRequest().PropertiesByAttributeAutowired();
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            registrar.As(toType).Named(name,toType).InstancePerRequest().PropertiesByAttributeAutowired();
                        }
                        else
                        {
                            registrar.As(toType).InstancePerRequest().PropertiesByAttributeAutowired();
                        }
                    }

                    break;
                case AutofacScope.InstancePerLifetimeScope:
                    if (toType == null)
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            registrar.Named(name,fromType).InstancePerLifetimeScope().PropertiesByAttributeAutowired();
                        }
                        else
                        {
                            registrar.InstancePerLifetimeScope().PropertiesByAttributeAutowired();
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            registrar.As(toType).Named(name,toType).InstancePerLifetimeScope().PropertiesByAttributeAutowired();
                        }
                        else
                        {
                            registrar.As(toType).InstancePerLifetimeScope().PropertiesByAttributeAutowired();
                        }
                    }

                    break;
                case AutofacScope.InstancePerMatchingLifetimeScope:
                    if (toType == null)
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            registrar.Named(name,fromType).InstancePerMatchingLifetimeScope().PropertiesByAttributeAutowired();
                        }
                        else
                        {
                            registrar.InstancePerMatchingLifetimeScope().PropertiesByAttributeAutowired();
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            registrar.As(toType).Named(name,toType).InstancePerMatchingLifetimeScope().PropertiesByAttributeAutowired();
                        }
                        else
                        {
                            registrar.As(toType).InstancePerMatchingLifetimeScope().PropertiesByAttributeAutowired();
                        }
                    }

                    break;
            }

            if (bean.InterceptorType!=null)
            {
                try
                {
                    registrar.EnableInterfaceInterceptors().InterceptedBy(bean.InterceptorType);
                }
                catch (Exception)
                {
                };
            }

            if (string.IsNullOrEmpty(name))
            {

                //默认使用类的名字注册
                var newBean = bean.Clone();
                newBean.Named = fromType.Name;
                BindBuildWithScope(builder,fromType,newBean);
            }
        }


        /// <summary>
        /// Set the policy used to find candidate properties on the implementation type.
        /// </summary>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TActivatorData">Activator data type.</typeparam>
        /// <typeparam name="TStyle">Registration style.</typeparam>
        /// <param name="registration">Registration to set policy on.</param>
        /// <param name="propertySelector">Policy to be used when searching for properties to inject.</param>
        /// <returns>A registration builder allowing further configuration of the component.</returns>
        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> PropertiesByAttributeAutowired<TLimit, TActivatorData, TStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration)
        {
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));
            bool AutowiredPropertySelector(PropertyInfo property, object o) => property.GetCustomAttributes<AutowiredAttribute>().Any();
            return registration.PropertiesAutowired((IPropertySelector) new DelegatePropertySelector(AutowiredPropertySelector), false);
        }
    }
}