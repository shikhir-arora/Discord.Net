// Handle Events using Lambdas
_client.MessageReceived += (s, e) =>
{
	if (!e.Message.IsAuthor)
		await e.Channel.SendMessage(e.Message.Text);
}

// Handle Events using Event Handlers
EventHandler<MessageEventArgs> handler = new EventHandler<MessageEventArgs>(HandleMessageCreated);
_client.MessageReceived += handler;

// Event Handler
void HandleMessageCreated(object sender, EventArgs e)
{
	if (!e.Message.IsAuthor)
		await e.Channel.SendMessage(e.Message.Text);
}