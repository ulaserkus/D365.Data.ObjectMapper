using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace D365.Data.ObjectMapper.Attributes
{
    ///<summary>
    ///<para>This Attribute Maps From EntityReference Object's Id Property To Class Property</para>
    ///<para>Used Property Type Must Be Guid</para>
    ///</summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ReferenceAttribute : Attribute
    {
        public string LogicalName { get; set; }

        public ReferenceAttribute(string logicalName, [CallerFilePath] string filePath = null)
        {
            LogicalName = logicalName.ToLower();

            string callerClassName = Path.GetFileNameWithoutExtension(filePath);
            var assembly = Assembly.GetEntryAssembly();
            Type type = assembly.GetTypes().Where(x => x.Name == callerClassName).FirstOrDefault();

            if (type != null)
            {
                foreach (var prop in type.GetProperties().ToList())
                {
                    IEnumerable<CustomAttributeData> customAttributeDatas = prop.CustomAttributes;

                    var primaryKeyAttributes = customAttributeDatas.Where(x => x.AttributeType.Name == "ReferenceAttribute").ToList();

                    if (primaryKeyAttributes != null && primaryKeyAttributes.Count == 1)
                    {
                        if (typeof(Guid) != prop.PropertyType)
                        {
                            throw new InvalidCastException("ReferenceAttribute property type must be Guid");
                        }
                    }

                }
            }
        }
    }
}
