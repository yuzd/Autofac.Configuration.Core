using System;
using System.Collections.Generic;
using System.Text;

namespace Autofac.Configuration.Attribute
{
    public enum AutofucScope
    {
        InstancePerDependency,
        SingleInstance,
        InstancePerLifetimeScope,
        InstancePerMatchingLifetimeScope,
        InstancePerRequest,
        InstancePerOwned
    }
}
