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

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            List<Type> types = new List<Type>();

            if (assemblies != null && assemblies.Length > 0)
            {
                foreach (var ass in assemblies)
                {
                    List<Type> type = ass.GetTypes().Where(x => x.Name == callerClassName).ToList();

                    if (type != null && type.Count > 0)
                        types.AddRange(type);
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
}
