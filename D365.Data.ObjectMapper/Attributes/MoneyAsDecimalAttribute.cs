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
            var assembly = Assembly.GetEntryAssembly();
            Type type = assembly.GetTypes().Where(x => x.Name == callerClassName).FirstOrDefault();

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
