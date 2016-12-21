using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GroupAttribute : Attribute
    {
        public string Prefix { get; }
        public string Name { get; }

        public GroupAttribute()
        {
            Prefix = null;
            Name = null;
        }
        public GroupAttribute(string prefix, string name)
        {
            Prefix = prefix;
            Name = name;
        }
    }
}
