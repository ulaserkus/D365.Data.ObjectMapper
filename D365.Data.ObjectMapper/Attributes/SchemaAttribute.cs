using System;

namespace D365.Data.ObjectMapper.Attributes
{
    ///<summary>
    ///<para>This Attribute Specifies your Entity Schema Name</para>
    ///</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class SchemaAttribute : Attribute
    {
        public string LogicalName { get; set; }

        public SchemaAttribute(string logicalName)
        {
            LogicalName = logicalName.ToLower();
        }
    }
}
