using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.Embed;

namespace Discord.Rest
{
    internal class Embed : IEmbed
    {
        public string Description { get; }
        public string Url { get; }
        public string Title { get; }
        public string Type { get; }
        public uint? Color { get; }
        public EmbedAuthor? Author { get; }
        public EmbedFooter? Footer { get; }
        public EmbedProvider? Provider { get; }
        public EmbedThumbnail? Thumbnail { get; }
        public ImmutableArray<EmbedField> Fields { get; }

        public Embed(Model model)
        {
            Url = model.Url;
            Type = model.Type;
            Title = model.Title;
            Description = model.Description;
            Color = model.Color;


            if (model.Provider.IsSpecified)
                Provider = new EmbedProvider(model.Provider.Value);
            if (model.Thumbnail.IsSpecified)
                Thumbnail = new EmbedThumbnail(model.Thumbnail.Value);
            if (model.Author.IsSpecified)
                Author = new EmbedAuthor(model.Author.Value);
            if (model.Footer.IsSpecified)
                Footer = new EmbedFooter(model.Footer.Value);
            if (model.Fields.IsSpecified)
                Fields = model.Fields.Value.Select(x => EmbedField.Create(x)).ToImmutableArray();
        }
    }
}
