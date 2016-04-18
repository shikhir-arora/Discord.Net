Commands
========

|incomplete|

The `Discord.Net.Commands`_ package DiscordBotClient extends DiscordClient with support for commands.

.. _Discord.Net.Commands: https://www.nuget.org/packages/Discord.Net.Commands

Setup
-----

To use Commands, you must first install the CommandService to your DiscordClient.

..code-block:: csharp6
	
	_client.UsingCommands(x => {
		x.CommandChar = '$';
		x.HelpMode = HelpMode.Public;
	});

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
