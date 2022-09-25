using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using D365.Data.ObjectMapper.Models;
using D365.Data.ObjectMapper.Attributes;

namespace D365.Data.ObjectMapper.Mapper
{
    internal class ObjectMapper<T> : IObjectMapper<T>
        where T : class, new()
    {
        #region Mapping
        public T Map(object entityOrEntityCollection)
        {
            try
            {
                ConvertableEntityInfo entity = (ConvertableEntityInfo)Activator.CreateInstance(typeof(ConvertableEntityInfo), entityOrEntityCollection);

                if (!string.IsNullOrEmpty(entity.AssemblyName) && entity.AssemblyName.Contains("Microsoft.Xrm.Sdk.Entity") && entity.ObjectName == "Entity")
                {
                    var ent = entity.EntityObject;

                    IEnumerable<object> list = (new T() as IEnumerable<object>);

                    if (list != null)
                    {
                        Type myListElementType = list.GetType().GetGenericArguments().SingleOrDefault();
                        Type d1 = list.GetType().GetGenericTypeDefinition();
                        Type[] typeArgs = { myListElementType };
                        Type makeme = d1.MakeGenericType(typeArgs);
                        object createdList = Activator.CreateInstance(makeme);
                        var element = Activator.CreateInstance(myListElementType);
                        MapEntity(ent, ref element);
                        var type = createdList.GetType();
                        var methodInfo = type.GetMethod("Add");

                        if (type.Name.Contains("Queue"))
                        {
                            methodInfo = type.GetMethod("Enqueue");
                        }
                        else if (type.Name.Contains("Stack"))
                        {
                            methodInfo = type.GetMethod("Push");
                        }

                        methodInfo.Invoke(createdList, new object[] { element });
                        return (T)createdList;
                    }

                    var obj = Activator.CreateInstance(typeof(T));

                    MapEntity(ent, ref obj);

                    return (T)obj;
                }
                else if (!string.IsNullOrEmpty(entity.AssemblyName) && entity.AssemblyName.Contains("Microsoft.Xrm.Sdk.EntityCollection") && entity.ObjectName == "EntityCollection")
                {
                    IEnumerable<object> list = (new T() as IEnumerable<object>);

                    if (list != null)
                    {
                        Type myListElementType = list.GetType().GetGenericArguments().SingleOrDefault();

                        var entities = entity.EntityObject.GetType().GetProperty("Entities").GetValue(entity.EntityObject, null) as IEnumerable<object>;

                        if (entities != null && entities.Count() > 0)
                        {
                            Type d1 = list.GetType().GetGenericTypeDefinition();
                            Type[] typeArgs = { myListElementType };
                            Type makeme = d1.MakeGenericType(typeArgs);
                            object createdList = Activator.CreateInstance(makeme);

                            foreach (var item in entities)
                            {
                                var element = Activator.CreateInstance(myListElementType);

                                MapEntity(item, ref element);

                                var type = createdList.GetType();

                                var methodInfo = type.GetMethod("Add");

                                if (type.Name.Contains("Queue"))
                                {
                                    methodInfo = type.GetMethod("Enqueue");
                                }
                                else if(type.Name.Contains("Stack"))
                                {
                                    methodInfo = type.GetMethod("Push");
                                }
                                
                                methodInfo.Invoke(createdList, new object[] { element });
                            }

                            return (T)createdList;
                        }

                        return (T)list;
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("you can't convert {0} to single object {1}", entity.ObjectName, typeof(T).Name));
                    }
                }
                else
                {
                    throw new InvalidOperationException(string.Format("{0} object is not convertable type", entity.ObjectName));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void MapEntity(object value, ref object target)
        {
            try
            {
                var attributes = value.GetType().GetProperty("Attributes").GetValue(value, null);

                var values = (attributes.GetType().GetProperty("Values").GetValue(attributes, null) as ICollection<object>).ToList();

                var keys = (attributes.GetType().GetProperty("Keys").GetValue(attributes, null) as ICollection<string>).ToList();

                foreach (PropertyInfo prop in target.GetType().GetProperties())
                {
                    PrimaryKeyAttribute PrimaryKeyAttribute = prop.GetCustomAttribute(typeof(PrimaryKeyAttribute)) as PrimaryKeyAttribute;

                    MapFromAttribute mapFrom = prop.GetCustomAttribute(typeof(MapFromAttribute)) as MapFromAttribute;

                    OptionSetAsIntAttribute mapFromOptSetValue = prop.GetCustomAttribute(typeof(OptionSetAsIntAttribute)) as OptionSetAsIntAttribute;

                    MoneyAsDecimalAttribute mapFromMoney = prop.GetCustomAttribute(typeof(MoneyAsDecimalAttribute)) as MoneyAsDecimalAttribute;

                    ReferenceAttribute reference = prop.GetCustomAttribute(typeof(ReferenceAttribute)) as ReferenceAttribute;

                    if (PrimaryKeyAttribute != null)
                    {
                        Guid id = (Guid)value.GetType().GetProperty("Id").GetValue(value, null);

                        prop.SetValue(target, id, null);
                    }
                    else
                    {
                        if (mapFrom != null)
                        {
                            var name = mapFrom.From;
                            var key = keys.Where(x => x == name).SingleOrDefault();
                            int index = keys.IndexOf(key);
                            var val = values.ElementAtOrDefault(index);

                            MapByAttributes(name, val, mapFromOptSetValue, mapFromMoney, reference, prop, ref target);
                        }
                        else
                        {
                            var name = prop.Name.ToLower();
                            var key = keys.Where(x => x == name).SingleOrDefault();
                            int index = keys.IndexOf(key);
                            var val = values.ElementAtOrDefault(index);

                            MapByAttributes(name, val, mapFromOptSetValue, mapFromMoney, reference, prop, ref target);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void MapByAttributes(string name, object val, OptionSetAsIntAttribute mapFromOptSetValue, MoneyAsDecimalAttribute mapFromMoney, ReferenceAttribute reference, PropertyInfo prop, ref object target)
        {
            try
            {
                if (!string.IsNullOrEmpty(name) && val != null)
                {
                    if (val != null)
                    {
                        if ((mapFromOptSetValue != null || mapFromMoney != null) && reference == null)
                            val = val.GetType().GetProperty("Value").GetValue(val, null);
                        else if (mapFromOptSetValue == null && mapFromMoney == null && reference != null)
                            val = val.GetType().GetProperty("Id").GetValue(val, null);
                        else if ((mapFromOptSetValue != null || mapFromMoney != null) && reference != null)
                            throw new InvalidOperationException("You can't you Reference attribute with MoneyAsDecimal attribute or OptionSetAsInt");

                        var convertedValue = Convert.ChangeType(val, prop.PropertyType);

                        if (convertedValue != null && convertedValue != default)
                        {
                            prop.SetValue(target, convertedValue, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Parsing Methods
        public bool TryParseEntity<TEntity>(T obj, out TEntity entity)
             where TEntity : class, new()
        {
            try
            {
                if (obj != null)
                {
                    entity = new TEntity();

                    ConvertableEntityInfo convertValues = (ConvertableEntityInfo)Activator.CreateInstance(typeof(ConvertableEntityInfo), entity);

                    if (!string.IsNullOrEmpty(convertValues.AssemblyName) && convertValues.AssemblyName.Contains("Microsoft.Xrm.Sdk.Entity") && convertValues.ObjectName == "Entity")
                    {
                        IEnumerable<object> list = (obj as IEnumerable<object>);

                        if (list != null)
                        {
                            throw new InvalidOperationException(string.Format("you can't convert {0} plural object to {1}", typeof(T).Name, convertValues.ObjectName));
                        }

                        MapObject(ref obj, ref entity);

                        return true;
                    }
                    else
                    {
                        throw new InvalidOperationException("you must be parse to Microsoft.Xrm.Sdk.Entity");
                    }
                }
                else
                {
                    throw new InvalidOperationException("obj can't be null");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void MapObject<TEntity>(ref T value, ref TEntity target)
           where TEntity : class, new()
        {
            try
            {
                var targetType = target.GetType();
                var assembly = targetType.Assembly;
                var valueType = value.GetType();
                var method = targetType.GetMethod("set_Item");
                var className = valueType.Name.ToLower();
                var contactProperties = valueType.GetProperties().ToList();
                var idProp = contactProperties.Where(x => x.GetCustomAttribute(typeof(PrimaryKeyAttribute)) != null).SingleOrDefault();

                SchemaAttribute entityTableAttribute = valueType.GetCustomAttribute(typeof(SchemaAttribute)) as SchemaAttribute;
                if (entityTableAttribute != null)
                    className = entityTableAttribute.LogicalName.ToLower();

                targetType.GetProperty("LogicalName").SetValue(target, className, null);

                if (idProp == null)
                {
                    try
                    {
                        var idAttribute = className + "id";
                        var IdAttribute = char.ToUpper(className[0]) + className.Substring(1) + "Id";
                        var IDATTRİBUTE = className.ToUpper() + "ID";

                        var IdProp = valueType.GetProperties().Where(id => id.Name == idAttribute || id.Name == IdAttribute || id.Name == IDATTRİBUTE).FirstOrDefault();

                        var classId = IdProp.GetValue(value, null);

                        Guid entityId = classId != null ? (Guid)classId : Guid.Empty;

                        targetType.GetProperty("Id").SetValue(target, entityId, null);
                    }
                    catch (NullReferenceException)
                    {
                        throw new Exception($"Entity id not found please check your properties  ex: {className + "id"},{char.ToUpper(className[0]) + className.Substring(1) + "Id"},{className.ToUpper() + "ID"}");
                    }
                }
                else
                {
                    Guid entityId = idProp != null ? (Guid)idProp.GetValue(value, null) : Guid.Empty;
                    targetType.GetProperty("Id").SetValue(target, entityId, null);
                    contactProperties.Remove(idProp);
                }

                foreach (PropertyInfo prop in contactProperties)
                {
                    MapFromAttribute mapFrom = prop.GetCustomAttribute(typeof(MapFromAttribute)) as MapFromAttribute;
                    OptionSetAsIntAttribute mapFromOptSetValue = prop.GetCustomAttribute(typeof(OptionSetAsIntAttribute)) as OptionSetAsIntAttribute;
                    MoneyAsDecimalAttribute mapFromMoney = prop.GetCustomAttribute(typeof(MoneyAsDecimalAttribute)) as MoneyAsDecimalAttribute;
                    ReferenceAttribute reference = prop.GetCustomAttribute(typeof(ReferenceAttribute)) as ReferenceAttribute;

                    var name = prop.Name.ToLower();
                    var val = prop.GetValue(value, null);

                    if (mapFrom == null)
                    {
                        SetEntityPropertiesTypeOfAttributes(val, name, reference, mapFromOptSetValue, mapFromMoney, method, assembly, target);
                    }
                    else
                    {
                        SetEntityPropertiesTypeOfAttributes(val, mapFrom.From, reference, mapFromOptSetValue, mapFromMoney, method, assembly, target);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetEntityPropertiesTypeOfAttributes(object value, string name, ReferenceAttribute reference, OptionSetAsIntAttribute mapFromOptSetValue, MoneyAsDecimalAttribute mapFromMoney, MethodInfo setMethod, Assembly XrmSdk, object target)
        {
            try
            {
                if (value != null && !string.IsNullOrEmpty(name))
                {
                    object valueSet = null;
                    object oldValue = value;

                    if (mapFromMoney != null && mapFromOptSetValue == null && reference == null)
                    {
                        decimal money = (decimal)oldValue;
                        valueSet = XrmSdk.CreateInstance("Microsoft.Xrm.Sdk.Money");
                        valueSet.GetType().GetProperty("Value").SetValue(valueSet, money);
                        setMethod.Invoke(target, new object[] { name.ToString(), valueSet });
                        return;
                    }
                    else if (mapFromOptSetValue != null && mapFromMoney == null && reference == null)
                    {
                        int option = (int)oldValue;
                        valueSet = XrmSdk.CreateInstance("Microsoft.Xrm.Sdk.OptionSetValue");
                        valueSet.GetType().GetProperty("Value").SetValue(valueSet, option);
                        setMethod.Invoke(target, new object[] { name.ToString(), valueSet });
                        return;
                    }
                    else if (mapFromOptSetValue == null && mapFromMoney == null && reference != null)
                    {
                        Guid id = (Guid)oldValue;
                        if (id != Guid.Empty)
                        {
                            valueSet = XrmSdk.CreateInstance("Microsoft.Xrm.Sdk.EntityReference");
                            valueSet.GetType().GetProperty("Id").SetValue(valueSet, id);
                            valueSet.GetType().GetProperty("LogicalName").SetValue(valueSet, reference.LogicalName);
                            setMethod.Invoke(target, new object[] { name.ToString(), valueSet });
                            return;
                        }
                    }
                    else if ((mapFromOptSetValue != null || mapFromMoney != null) && reference != null)
                    {
                        throw new InvalidOperationException("You can't use MoneyAsDecimal, OptionSetAsInt,Reference attributes with");
                    }
                    else
                    {
                        valueSet = oldValue;
                    }

                    setMethod.Invoke(target, new object[] { name.ToString(), valueSet });
                }
                else if (value == null && !string.IsNullOrEmpty(name))
                {
                    setMethod.Invoke(target, new object[] { name.ToString(), value });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}
