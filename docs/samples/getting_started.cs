using Discord;

class Program
{
	static void Main(string[] args) => new Program().Run().GetAwaiter().GetResult();
	
	private DiscordSocketClient _client;

	public async Task Run()
	{
		_client = new DiscordSocketClient();

		string token = "aaabbbccc";

		_client.MessageReceived += async (message) =>
        {
            if (!(message.Author.Id == (await _client.GetCurrentUserAsync()).Id))
                await message.Channel.SendMessageAsync(message.Text);
        };

		await _client.LoginAsync(TokenType.Bot, token);
		await _client.ConnectAsync();

		Console.Read();
	}
}
