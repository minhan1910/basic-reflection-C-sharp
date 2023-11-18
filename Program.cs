using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReflectionDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Type personType = typeof(Person);
            //Console.WriteLine(personType.Name);
            //Console.WriteLine(personType.Assembly);
            //Console.WriteLine(personType.FullName);
            //Console.WriteLine(personType.BaseType);

            //Console.WriteLine("\n");
            //Console.WriteLine("Properties: ");
            //personType.GetProperties().ToList().ForEach(prop =>
            //{
            //    Console.WriteLine($"Property types: {prop.PropertyType.Name} | Property Name: {prop.Name}");
            //});

            //Console.WriteLine("\n\n");
            //Console.WriteLine("Methods: ");
            //var person = new Person();
            //person.FirstName = "Test";
            //person.LastName = "An";
            //personType.GetMethods().ToList().ForEach(method =>
            //{
            //    Console.WriteLine($"Method: Type: {method.ReturnType.Name} | Name: {method.Name}");
            //    List<object> methodParams = new List<object>();

            //    if (method.Name == "Print")
            //    {                    
            //        method.Invoke(person, methodParams.ToArray());
            //    }
            //});
                   
            TestHelper.Test(typeof(Person));
        }
    }

    public class People { }

    public class Person : People
    {
        public string FirstName { get; set; } = "Test";
        public string LastName { get; set; } = "Test";
        public string Phone { get; set; }
        public int ZipCode { get; set; }

        [MethodForRun(RunCount = 3)]
        public void Print()
        {
            Console.WriteLine($"{FirstName} {LastName}");
        }

        [MethodForRun(RunCount = 10)]
        public void Print2()
        {
            Console.WriteLine($"Print2 invoked");
        }

        [MethodForRun(RunCount = 1)]
        public void SayHi()
        {
            Console.WriteLine($"Hi {FirstName}");
        }

        public void Move(int newZipCode)
        {
            ZipCode = newZipCode;
            Console.WriteLine($"{FirstName} {LastName} has been moved to {ZipCode}");
        }
    }

    class TestHelper
    {
        public static void AttributeTest(object objectTest, Type type)
        {
            var allMethods = type.GetMethods();

            foreach (var method in allMethods)
            {
                if (method.Name.StartsWith("set_") || method.Name.StartsWith("get_"))
                {
                    continue;
                }

                var methodAttributes = method.GetCustomAttributes(true);

                foreach (var attribute in methodAttributes)
                {
                    if (attribute.GetType().Name == nameof(MethodForRunAttribute))
                    {
                        //var attributeValues = method.GetCustomAttributesData();
                        var attributeValue = attribute as MethodForRunAttribute;
                        var runCnt = attributeValue.RunCount;
                        while (runCnt != 0)
                        {
                            if (method.ReturnType.Name == "Void")
                            {
                                method.Invoke(objectTest, null);
                            }
                            --runCnt;
                        }
                    }
                }
            }
        }
           
        public static void Test(Type type)
        {
            var allMethods = type.GetMethods();
            var allAttributeMethods = allMethods.Where(m => m.GetCustomAttribute(typeof(MethodForRunAttribute)) != null);

            var objTest = Activator.CreateInstance(type);

            foreach (var attributeMethod in allAttributeMethods)
            {
                var attributeValue = attributeMethod.GetCustomAttribute(typeof(MethodForRunAttribute)) as MethodForRunAttribute;
                var runCnt = attributeValue.RunCount;
                while (runCnt-- != 0)
                {
                    attributeMethod.Invoke(objTest, null);  
                }
            }
        }
    }

    public class MethodForRunAttribute : Attribute 
    {
        public int RunCount { get; set; }
    }
}
