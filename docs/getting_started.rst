Getting Started
===============

Requirements
------------

Discord.Net currently requires logging in with a bot account. You can `register a bot account here`_.

Bot Accounts must be added to a server, you must use the `OAuth 2 Flow`_ to add them to servers.

.. _register a bot account here: https://discordapp.com/developers/applications/me
.. _OAuth 2 Flow: https://discordapp.com/developers/docs/topics/oauth2#adding-bots-to-guilds

Installation
------------

~~You can get Discord.Net from NuGet:~~

Leaving this as a placeholder, however **you must compile from source.**

* `Discord.Net`_

If you have trouble installing from NuGet, try installing dependencies manually.

.. warning::
	
	The latest versions of Discord.Net on NuGet are outdated, and you may run into performance issues. It is reccomended that you pull the latest source from `GitHub`_

You can also pull the latest source from `GitHub`_ 

As an alternative, precompiled binaries are available on our `Continuous Integration`_ server.

.. _Discord.Net: https://www.nuget.org/packages/Discord.Net
.. _Discord.Net.Commands: https://www.nuget.org/packages/Discord.Net.Commands
.. _Discord.Net.Modules: https://www.nuget.org/packages/Discord.Net.Modules
.. _Discord.Net.Modules: https://www.nuget.org/packages/Discord.Net.Audio
.. _GitHub: https://github.com/RogueException/Discord.Net/
.. _Continuous Integration: https://ci.appveyor.com/project/foxbot/discord-net/history

Async
-----

Discord.Net uses C# tasks extensively - nearly all operations return one. It is highly recommended that these tasks be awaited whenever possible.
To do so requires the calling method be marked as async, which can be problematic in a console application. An example of how to get around this is provided below.

For more information, go to `MSDN's Await-Async section`_.

.. _MSDN's Await-Async section: https://msdn.microsoft.com/en-us/library/hh191443.aspx

First Steps
-----------
   
.. literalinclude:: samples/getting_started.cs
   :language: csharp6
   :tab-width: 2

First Steps Annotated
---------------------

The above example will be enough for you to create a basic Echo Bot. Keep in mind, echo bots are discouraged in public servers, so make sure your bot is only in your testing server.\

That might have been a lot, so let's go through each line.

``using Discord;`` - This is the first line, and it declares that we will be using the Discord.Net API in our class.

Next, we create the Program class, as you would generally do in C#.

``static void Main(string[] args) => new Program().Run().GetAwaiter().GetResult();`` - It's not good practice to run all of your code in one static method, so we create a new non-static instance of ``Program`` and run the ``Run`` task. We use ``GetAwaiter().GetResult();`` because ``Run`` is an async Task, and we want our program to block until that task is completed.

``private DiscordSocketClient _client;`` - Here we define the main ``DiscordSocketClient`` that we will be using in our project. It's standard convention to name the private DiscordClient ``_client``, and I encourage that you do so also.

.. note:: 
	In versions before 1.0, you would normally use ``DiscordClient``, and you may notice that ``DiscordClient`` still exists. However, the base DiscordClient functions only to make REST calls, and cannot perform many major functions of a Discord Bot, such as receiving messages.

``public async Task Run()`` - As explained above, it's not good practice to run everything out of Main, so here is our new Main method. We mark this as an ``async Task`` so that we can run asynchronus code inside of it.

``_client = new DiscordSocketClient();`` - Here, we define ``_client`` as a new ``DiscordSocketClient``, so we can begin to use it.

``string token = "aaabbbccc";`` Here, we are storing our token for use in the login method. There is no advantage to storing your token into a string here, except for that it helps to prevent clutter.

``_client.MessageReceived += async (message) => {`` - This is a lambda, a feature which allows us to define functions or handlers inline, without creating a new method. Here, we are hooking into the ``MessageReceived`` "event" on the DiscordSocketClient. The ``async (message)`` indicates that the lambda will be an async function, and we are passing a single parameter, message into it. While yhou are subscribing to an event, you are not limited to the standard EventHandler pattern, and you no longer need to accept ``sender`` as an argument.

``if (!(message.Author.Id == (await _client.GetCurrentUserAsync()).Id))`` - This ensures that we did not create the message that was received. This helps to keep us from creating an infinite echo bot.

``await message.Channel.SendMessageAsync(Message.Text)`` - Here, we are sending a message to the channel the message was received in. The contents of the message we are sending is identical to that of the message we received.

``};`` - Close up the lambda

``await _client.LoginAsync(TokenType.Bot, token)`` In 1.0, you now need to call a login function and pass in your token. 

``await _client.ConnectAsync();`` - Finally, we connect the bot to Discord. 

.. warning::
	In previous versions of Discord.Net, you had to hook into the Ready and GuildAvailable events separately before you could ensure your bot was online. 

	In 1.0, the ``ConnectAsync`` method does not return until the ``Ready`` event has been processed. By default, the ``ConnectAsync`` method also waits until all guilds have been streamed in. You can disable this feature by passing ``false`` into ``ConnectAsync``.