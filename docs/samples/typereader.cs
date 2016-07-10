// Untested
public class BooleanTypeReader : TypeReader
{
	public override Task<TypeReaderResult> Read(IMessage context, string input)
	{
		IGuildChannel guildChannel = context.Channel as IGuildChannel;
		
		bool result;
		if (bool.TryParse(input, out result))
		{
			return Task.FromResult(TypeReaderResult.FromSuccess(result));
		}
		
		// TODO: Correct this!
		return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Input could not be parsed as a boolean.")) 
	}
}