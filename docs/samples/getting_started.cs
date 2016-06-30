using Discord;

class Program
{
	static void Main(string[] args) => new Program().Run().GetAwaiter().GetResult();
	
	private DiscordSocketClient _client;

	public async Task Run()
	{
		_client = new DiscordSocketClient();

		string token = "aaabbbccc";

		// To create commands the /right/ way, please see the 'commands' section.
		_client.MessageReceived += async (message) =>
        {
            if (message.Text == "!ping")
            	await message.Channel.SendMessageAsync("pong");
        };

		await _client.LoginAsync(TokenType.Bot, token);
		await _client.ConnectAsync();

		Console.Read();
	}
}
