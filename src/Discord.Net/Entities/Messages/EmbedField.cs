using Model = Discord.API.EmbedField;

namespace Discord
{
    public struct EmbedField
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Inline { get; set; }

        public EmbedField(string name, string value, bool inline)
        {
            Name = name;
            Value = value;
            Inline = inline;
        }
        internal static EmbedField Create(Model model)
        {
            return new EmbedField(model.Name, model.Value, model.Inline);
        }
        internal EmbedField(Model model)
            : this(model.Name, model.Value, model.Inline) { }
    }
}
