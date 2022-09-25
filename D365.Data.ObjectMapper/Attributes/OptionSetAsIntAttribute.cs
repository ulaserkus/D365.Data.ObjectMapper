using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace D365.Data.ObjectMapper.Attributes
{
    ///<summary>
    ///<para>This Attribute Parse OptionSetValue Object's Value Property To Class Property</para>
    ///<para>Class Property Type Must Be Int</para>
    ///</summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class OptionSetAsIntAttribute : Attribute
    {
        public OptionSetAsIntAttribute([CallerFilePath] string filePath = null)
        {
            string callerClassName = Path.GetFileNameWithoutExtension(filePath);
            var assembly = Assembly.GetEntryAssembly();
            Type type = assembly.GetTypes().Where(x => x.Name == callerClassName).FirstOrDefault();

            if (type != null)
            {
                foreach (var prop in type.GetProperties().ToList())
                {
                    IEnumerable<CustomAttributeData> customAttributeDatas = prop.CustomAttributes;

                    var KeyAttributes = customAttributeDatas.Where(x => x.AttributeType.Name == "OptionSetAsIntAttribute").ToList();

                    if (KeyAttributes != null && KeyAttributes.Count == 1)
                    {
                        if (typeof(int) != prop.PropertyType)
                        {
                            throw new InvalidCastException("OptionSetAsIntAttribute property type must be int");
                        }
                    }

                }
            }
        }
    }
}
