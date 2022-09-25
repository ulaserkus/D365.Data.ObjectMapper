using D365.Data.ObjectMapper.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace D365.Data.ObjectMapper
{
    public static class XrmMapper
    {
        internal static Dictionary<Type, object> Configs;

        ///<summary>
        ///<para>This Method Mapping Microsoft.Xrm.Sdk.Entity Or Microsoft.Xrm.Sdk.EntityCollection To Your Custom Classes or Generic Collections</para>
        ///</summary>
        public static Target Map<Target>(object entityOrEntityCollection)
             where Target : class, new()
        {
            try
            {
                if (Configs is null)
                    Configs = new Dictionary<Type, object>();

                var type = typeof(Target);

                if (Configs.TryGetValue(type, out object objectMapper))
                {
                    IObjectMapper<Target> mapper = (IObjectMapper<Target>)objectMapper;

                    return mapper.Map(entityOrEntityCollection);
                }
                else
                {
                    IObjectMapper<Target> mapper = new ObjectMapper<Target>();

                    Configs.Add(typeof(Target), mapper);

                    return mapper.Map(entityOrEntityCollection);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///<summary>
        ///<para>This Method Parsing Your Custom Classes To Microsoft.Xrm.Sdk.Entity</para>
        ///<para>Target Not Accept Collections It Will Throw Error </para>
        ///</summary>
        public static object ParseToEntity(this object target)
        {
            try
            {
                if (Configs is null)
                    Configs = new Dictionary<Type, object>();

                var type = target.GetType();

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                var assembly = assemblies.Where(x => x.GetName().Name == "Microsoft.Xrm.Sdk").FirstOrDefault();

                if (assembly == null)
                    throw new DllNotFoundException("Microsoft.Xrm.Sdk not found in used context");

                var tentity = assembly.CreateInstance("Microsoft.Xrm.Sdk.Entity");

                if (Configs.TryGetValue(type, out object objectMapper))
                {
                    MethodInfo methodTryParseEntity = objectMapper.GetType().GetMethod("TryParseEntity");
                    methodTryParseEntity = methodTryParseEntity.MakeGenericMethod(tentity.GetType());
                    object[] parameters = { target, null };
                    bool status = (bool)methodTryParseEntity.Invoke(objectMapper, parameters);

                    if (status == true)
                        return parameters[1];
                }
                else
                {
                    Type d1 = typeof(ObjectMapper<>);
                    Type[] typeArgs = { target.GetType() };
                    Type makeme = d1.MakeGenericType(typeArgs);
                    object createdObjectMapper = Activator.CreateInstance(makeme);

                    Configs.Add(type, createdObjectMapper);

                    MethodInfo methodTryParseEntity = createdObjectMapper.GetType().GetMethod("TryParseEntity");
                    methodTryParseEntity = methodTryParseEntity.MakeGenericMethod(tentity.GetType());
                    object[] parameters = { target, null };
                    bool status = (bool)methodTryParseEntity.Invoke(createdObjectMapper, parameters);

                    if (status == true)
                        return parameters[1];
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
