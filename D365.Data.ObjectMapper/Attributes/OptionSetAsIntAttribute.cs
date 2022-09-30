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

            List<Type> types = new List<Type>();

            if (assemblies != null && assemblies.Count > 0)
            {
                foreach (var ass in assemblies)
                {
                    Type type = ass.GetTypes().Where(x => x.Name == callerClassName).FirstOrDefault();

                    if (type != null)
                        types.Add(type);
                }
            }

            if (types != null && types.Count > 0)
            {
                foreach (Type type in types)
                {
                    foreach (var prop in type.GetProperties().ToList())
                    {
                        IEnumerable<CustomAttributeData> customAttributeDatas = prop.CustomAttributes;

                        var KeyAttributes = customAttributeDatas.Where(x => x.AttributeType.Name == "OptionSetAsIntAttribute").ToList();

                        if (KeyAttributes != null && KeyAttributes.Count > 0)
                        {
                            if (typeof(decimal) != prop.PropertyType)
                            {
                                throw new InvalidCastException("OptionSetAsIntAttribute property type must be int");
                            }
                        }

                    }
                }
            }
        }
    }
}
