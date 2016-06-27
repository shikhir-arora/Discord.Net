DiscordClient
=============

|outdated|

.. warning::

	I've decided this is a horrible idea to make, considering that we have the `MSDN Docs`_. I'm just leaving the page here in case I change my mind.

.. _MSDN Docs: http://discord.foxbot.me/docs/

|incomplete|

The ``DiscordClient`` is the main class for Discord.Net

.. function:: _client.Connect(string email, string password)

	Connects to Discord using the passed in email address and password

	**Return Value:** ``Task<string>`` - The Task Result is the user's token.

.. function:: _client.Connect(string token)
	
	Connects to Discord with a token, rather than using an email/password.

.. function:: _client.CreatePrivateChannel(ulong id)
	
	Creates a new Private Message Channel with a user, or returns an existing one. You must be in a server with the specified user, otherwise the function will throw an HttpException.

	**Return Value:** ``Task<Channel>`` - The Task Result is the Private Message Channel

.. function:: _client.CreateServer(string name, Discord.Region region, [Discord.ImageType imageType], [System.IO.Stream icon])
	
	Creates a new Server.

	**name**: The name of the server, as a ``string``.  
	**name**: The Region the server will be created in, using the ``Discord.Region`` enum.  
	*imageType*: The type of image you will be uploading (assumes the image parameter is also set), as a ``Discord.ImageType`` enum.
	*icon*: A IOStream pointing to an image you will use as the server's icon.  

	**Return Value:** ``Task<Server>`` - The Task Result is the Server

.. function:: _client.Disconnect();
	
	Disconnects from Discord.

.. function:: _client.ExecuteAndWait(Func<Task> action)
	
	Provides a wrapper to run async functions in a console app. Blocks the current thread until the DiscordClient disconnects. 

.. function:: _client.FindServers(string name)
	
	Finds servers in the DiscordClient based on ID

	**Return Value:** ``IEnumerable<Server>`` - An IEnumerable of Servers matching the query string