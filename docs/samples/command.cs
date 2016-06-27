using System.Threading.Tasks;
using Discord;
using Discord.Commands;

// Flag our SampleModule class with the 'Module' attribute, so the Command Service
// will automatically discover it and add its commands.
[Module]
public class SampleModule
{
	// Input: ~bot say hello --> hello
	[Command("say"), Description("Echos a message.")]
	public async Task Say(IMessage msg,
		[Unparsed, Description("The text to echo")] string echo)
	{
		await msg.Channel.SendMessageAsync(echo);
	}

	// ~bot square 20 --> 20 squared: 400
	[Command("square"), Description("Squares a number.")]
	public async Task Square(IMessage msg,
		[Description("The number to square.")] int number)
	{
		var result = Math.Pow(number, 2);
		await msg.Channel.SendMessageAsync($"{number} squared: {result}");
	}

	// ~userinfo --> foxbot#0282
	// ~userinfo @Khionu --> Khionu#8708 (Problems with the lib? DM Khionu#8708 for support)
	// ~userinfo Khionu#8708 --> Khionu#8708
	// ~userinfo Khionu --> Khionu#8708
	// ~userinfo 96642168176807936 --> Khionu#8708
	[Command("userinfo"), Description("Returns info about the current user, or the user parameter, if one passed.")]
    public async Task UserInfo(IMessage msg,
        [Description("The (optional) user to get info for")] IUser user = null)
    {
        var userInfo = user ?? await Program.Client.GetCurrentUserAsync();
        await msg.Channel.SendMessageAsync($"{userInfo.Username}#{userInfo.Discriminator}");
    }
}
