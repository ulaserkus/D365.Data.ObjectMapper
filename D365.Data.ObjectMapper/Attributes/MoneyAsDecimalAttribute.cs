using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace D365.Data.ObjectMapper.Attributes
{
    ///<summary>
    ///<para>This Attribute parse Money Object's Value Property To Class Property</para>
    ///<para>Class Property Type Must Be Decimal</para>
    ///</summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class MoneyAsDecimalAttribute : Attribute
    {
        public MoneyAsDecimalAttribute([CallerFilePath] string filePath = null)
        {
            string callerClassName = Path.GetFileNameWithoutExtension(filePath);

            List<Assembly> assemblies = new List<Assembly>();

            var assembly1 = Assembly.GetEntryAssembly();
            if (assembly1 != null)
                assemblies.Add(assembly1);

            var assembly2 = Assembly.GetExecutingAssembly();
            if (assembly2 != null)
                assemblies.Add(assembly2);

            var assembly3 = Assembly.GetCallingAssembly();
            if (assembly3 != null)
                assemblies.Add(assembly3);

            Type type = null;

            foreach (var ass in assemblies)
            {
                type = ass.GetTypes().Where(x => x.Name == callerClassName).FirstOrDefault();

                if (type != null)
                    break;
            }

            if (type != null)
            {
                foreach (var prop in type.GetProperties().ToList())
                {
                    IEnumerable<CustomAttributeData> customAttributeDatas = prop.CustomAttributes;

                    var KeyAttributes = customAttributeDatas.Where(x => x.AttributeType.Name == "MoneyAsDecimalAttribute").ToList();

                    if (KeyAttributes != null && KeyAttributes.Count == 1)
                    {
                        if (typeof(decimal) != prop.PropertyType)
                        {
                            throw new InvalidCastException("MoneyAsDecimalAttribute property type must be decimal");
                        }
                    }

                }
            }
        }
    }
}
