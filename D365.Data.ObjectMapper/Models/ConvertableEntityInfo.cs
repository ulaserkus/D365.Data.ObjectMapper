using System;

namespace D365.Data.ObjectMapper.Models
{
    internal class ConvertableEntityInfo
    {
        public Type ObjectType { get; private set; }

        public string ObjectName { get; private set; }

        public string AssemblyName { get; private set; }

        public object EntityObject { get; private set; }

        public ConvertableEntityInfo(object EntityObjects)
        {
            if (EntityObjects != null)
            {
                EntityObject = EntityObjects;

                ObjectType = EntityObjects.GetType();

                ObjectName = EntityObjects.GetType().Name;

                AssemblyName = EntityObject.GetType().AssemblyQualifiedName;
            }
            else
            {
                throw new Exception("Entity object or objects can't be null");
            }
        }
    }
}
