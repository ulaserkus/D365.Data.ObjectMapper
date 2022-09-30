using System;

namespace D365.Data.ObjectMapper.Attributes
{
    ///<summary>
    ///<para>This Attribute Maps From Specified Entity Attribute Values To Class Property</para>
    ///<para>You Can Use Any Type But Custom Class Property Type Need To Same Entity Attribute Type</para>
    ///</summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class MapFromAttribute : Attribute
    {
        public string From { get; set; }

        public MapFromAttribute(string from)
        {
            From = from.ToLower();
        }
    }
}
