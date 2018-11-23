using System;
using System.Reflection;
using Autofac.Builder;
using Autofac.Core;

namespace Autofac.Configuration
{
    /// <summary>
    /// Module for configuration parsing and registration.
    /// </summary>
    public class AutofucAttributeModule : Module
    {
        private Assembly[] _assemblyList;
        public AutofucAttributeModule(params Assembly[] assemblyList)
        {
            if (assemblyList.Length < 1)
            {
                throw new ArgumentException(nameof(assemblyList));
            }

            _assemblyList = assemblyList;
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

        }
    }

    public static class AutofucAttributeExtentions
    {
        /// <summary>
        /// Configure the component so that any properties whose types are registered in the
        /// container will be wired to instances of the appropriate service.
        /// </summary>
        /// <param name="registration">Registration to auto-wire properties.</param>
        /// <param name="wiringFlags">Set wiring options such as circular dependency wiring support.</param>
        /// <returns>A registration builder allowing further configuration of the component.</returns>
        public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> PropertiesByAttributeAutowired<TLimit, TActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registration, PropertyWiringOptions wiringFlags = PropertyWiringOptions.None)
        {
            bool preserveSetValues = (uint)(wiringFlags & PropertyWiringOptions.PreserveSetValues) > 0U;
            bool allowCircularDependencies = (uint)(wiringFlags & PropertyWiringOptions.AllowCircularDependencies) > 0U;
            return registration.PropertiesAutowired((IPropertySelector)new DefaultPropertySelector(preserveSetValues), allowCircularDependencies);
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
        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> PropertiesByAttributeAutowired<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, Func<PropertyInfo, object, bool> propertySelector)
        {
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));
            return registration.PropertiesAutowired((IPropertySelector)new DelegatePropertySelector(propertySelector), false);
        }
    }
}
