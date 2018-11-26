using System;
using Autofac;
using Autofac.Configuration;

namespace TestAutofac
{
    class Program
    {
        static void Main(string[] args)
        {

            var builder = new ContainerBuilder();
            //autofac打标签模式
            builder.RegisterModule(new AutofacAttributeModule(typeof(Program).Assembly));

            var container = builder.Build();

            var a1 = container.Resolve<Isay>();
            var a2 = container.ResolveNamed<Parent>(nameof(Parent));
            var a22 = container.Resolve<Parent>();
            var a3 = container.Resolve<Meal>();
            var a4 = container.Resolve<Child>();

            Console.WriteLine("Hello World!");
        }
    }
}