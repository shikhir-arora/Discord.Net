Getting Started
===============

Requirements
------------

Discord.Net currently requires logging in with a user account, however Discord will soon require the use of Bot Accounts. You can `register a bot account here`_.

Bot Accounts must be added to a server, you must use the `OAuth 2 Flow`_ to add them to servers.

.. _register a bot account here: https://discordapp.com/developers/applications/me
.. _OAuth 2 Flow: https://discordapp.com/developers/docs/topics/oauth2#adding-bots-to-guilds

Installation
------------

You can get Discord.Net from NuGet:

* `Discord.Net`_
* `Discord.Net.Commands`_
* `Discord.Net.Modules`_
* `Discord.Net.Audio`_

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

``static void Main(string[] args) => new Program().Start();`` - It's not good practice to run all of your code in one static method, so we create a new non-static instance of ``Program`` and run the ``Start`` method.

``private DiscordClient _client;`` - Here we define the main ``DiscordClient`` that we will be using in our project. It's standard convention to name the private DiscordClient ``_client``, and I encourage that you do so also.

``public void Start()`` - As explained above, it's not good practice to run everything out of Main, so here is our new Main method.

``_client = new DiscordClient();`` - Here, we define ``_client`` as a new ``DiscordClient``, so we can begin to use it.

``_client.MessageReceived += async (s, e) => {`` - This is a lambda, a feature which allows us to define functions or handlers inline, without creating a new method. Here, we are hooking into the ``MessageReceived`` event on the DiscordClient. The ``async (s, e)`` indicates that the lambda will be an async function, and we are passing two parameters, s(ender) and e(vent args) into it.

``if (!e.Message.IsAuthor)`` - This ensures that we did not create the message that was received. This helps to keep us from creating an infinite echo bot.

``await e.Channel.SendMessage(e.Message.Text)`` - Here, we are sending a message to the channel the message was received in. The contents of the message we are sending is identical to that of the message we received.

``};`` - Close up the lambda

``_client.ExecuteAndWait(async () => {`` - This invokes the ``ExecuteAndWait`` function of the DiscordClient, which allows us to run async code in a non-async method, and block the Console app until the Discord Client disconnects. Inside this function, we are creating an async lambda with no parameters (ExecuteAndWait takes an ``Action``, so we cannot use and parameters).

``await _client.Connect("aaabbbccc");`` - Next, we are going to connect our client, using the bot token that Discord provides us. If you are unsure of how to access your token, `see this image`_.

Finally, we close up our lambdas and program.

.. _see this image: http://i.foxbot.me/4rj5u.png