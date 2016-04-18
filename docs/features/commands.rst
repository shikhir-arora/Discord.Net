Commands
========

|incomplete|

The `Discord.Net.Commands`_ package DiscordBotClient extends DiscordClient with support for commands.

.. _Discord.Net.Commands: https://www.nuget.org/packages/Discord.Net.Commands

Setup
-----

To use Commands, you must first install the CommandService to your DiscordClient.

.. code-block:: csharp6
	
	_client.UsingCommands(x => {
		x.PrefixChar = '$';
		x.HelpMode = HelpMode.Public;
	});

By default, your bot will also respond to @mentions. It is reccomended you leave this feature enabled, and in crowded servers, require a mention to trigger your bot.

Example (Simple)
----------------

.. literalinclude:: /samples/command.cs
   :language: csharp6
   :tab-width: 2

Example (Groups)
----------------

.. literalinclude:: /samples/command_group.cs
   :language: csharp6
   :tab-width: 2
