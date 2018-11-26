using System;
using Autofac.Configuration.Attribute;
using Autofac.Core;

namespace TestAutofac
{
    [Bean]
    public class Meal
    {

    }

    [Bean]
    public class Parent
    {
        /// <summary>
        /// https://stackoverflow.com/questions/15600440/how-to-use-property-injection-with-autofac
        /// </summary>
        [Autowired]

        public Meal Child { get; set; }
    }


    public class Child:Meal,Isay
    {
        public void sayHello()
        {
            Console.WriteLine("child say ");
        }
    }


    public interface Isay
    {
        void sayHello();
    }

    public class ChineseSay : Isay
    {
        public void sayHello()
        {
            Console.WriteLine("hell ");
        }
    }
}