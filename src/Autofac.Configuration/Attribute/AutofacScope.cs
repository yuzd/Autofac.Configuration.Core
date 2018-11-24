using System;
using System.Collections.Generic;
using System.Text;

namespace Autofac.Configuration.Attribute
{
    public enum AutofacScope
    {
        InstancePerDependency,
        SingleInstance,
        InstancePerLifetimeScope,
        InstancePerMatchingLifetimeScope,
        InstancePerRequest,
        InstancePerOwned
    }
}
