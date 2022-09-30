using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Linq;

namespace D365.Data.ObjectMapper.Attributes
{
    ///<summary>
    ///<para>This Attribute Maps From Entity Object's Id Property To Class Property</para>
    ///<para>Class Property Type Must Be Guid And Unique Use Only Once Per Class</para>
    ///</summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute([CallerFilePath] string filePath = null)
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

            foreach (var ass in assemblies)
            {
                Type type = ass.GetTypes().Where(x => x.Name == callerClassName).FirstOrDefault();

                if (type != null)
                    types.Add(type);
            }

            if (types != null && types.Count > 0)
            {
                foreach (var type in types)
                {
                    int primaryKeyAttributeCount = 0;
                    var customAttributes = type.GetProperties().Select(x => x.CustomAttributes);
                    customAttributes.ToList().ForEach(y =>
                    {
                        primaryKeyAttributeCount += y.Where(x => x.AttributeType.Name == "PrimaryKeyAttribute").ToList().Count;
                    });

                    if (primaryKeyAttributeCount > 1)
                    {
                        throw new TargetParameterCountException("You can't use PrimaryKeyAttribute over than 1 in a same object");
                    }

                    foreach (var prop in type.GetProperties().ToList())
                    {
                        IEnumerable<CustomAttributeData> customAttributeDatas = prop.CustomAttributes;

                        var primaryKeyAttributes = customAttributeDatas.Where(x => x.AttributeType.Name == "PrimaryKeyAttribute").ToList();

                        if (primaryKeyAttributes != null && primaryKeyAttributes.Count > 0)
                        {
                            if (typeof(Guid) != prop.PropertyType)
                            {
                                throw new InvalidCastException("PrimaryKeyAttribute property type must be Guid");
                            }
                        }

                    }
                }
            }
        }
    }
}


