1.0 Conversion Guide
====================

This page is to help ease the transition between 0.9 and 1.0. Currently, 1.0 (dev) only supports REST only mode. All of the Documentation Below is for REST Only mode.

Client Modes
------------

**REST Only**

REST Only Mode is a lightweight version of Discord.Net that does not leverage Discord's WebSocket interface. It can only be used for retrieving information about channels, users, or guilds; sending messages to channels or users; and modifying some of the properties of guilds or channels.

A REST Only ``IDiscordClient`` can be created with the ``DiscordRestClient``.

**WebSockets**

Support for WebSockets have not been released yet, and as such, you cannot use any of the WebSocket features in 1.0 yet.

Interfaces
----------

Discord.Net 1.0 is heavily Interface Driven. The interfaces you can use are listed below.

.. function:: IDiscordClient
	
	The IDiscordClient is an Interface representing a DiscordClient. Currently, the only members that implement IDiscordClient is the ``DiscordRestClient`` (See Above).

Guilds
------

Guild Interfaces are all in relation to a Server on Discord.

.. function:: IGuild
	
	Represents a Guild.

	**0.9:** ``Server``

.. function:: IUserGuild
	
	Light object that represents a Guild a User is in. (Reference ``/users/@me/guilds``).

..function:: IGuildEmbed
	
	Represents a Guild Embed. (Reference ``/guilds/:id/embed``)

..function:: IVoiceRegion

	Represents a Voice Region.

	**0.9:** ``Region``

..function:: IGuildIntegration
	
	An integration with a third-party service, e.g. Twitch or Youtube Gaming.

..function:: IIntegrationAccount
	
	The account an ``IGuildIntegration`` is linked with.

Invites
-------

.. function:: IInvite
	
	Represents an Invite, as queried directly.

.. function:: IGuildInvite
	
	Represents an Invite, as queried from within a Guild.

.. function:: IPublicInvite
	
	|stub|


Channels
--------

Channel Interfaces model different types of Channels.

.. function:: IChannel
	
	Represents the most basic form of a Channel, leading into ``IGuildChannel`` and ``IDMChannel``.

.. function:: IGuildChannel
	
	Models a Channel Object that can be queried from a Guild. 

	**0.9:** ``Channel``

.. function:: IDMChannel
	
	Models a Channel Object that does not belong to a Guild, but rather another User.

	**0.9:** ``Channel.IsPrivate == True``

.. function:: IMessageChannel
	
	Represents a Channel that users can send messages to. Does not explicitly inherit from ``IGuildChannel`` or ``IDMChannel``.

.. function:: ITextChannel
	
	Represents a Text Channel within a Guild. 

	**0.9:** ``ChannelType.Text``

.. function:: IVoiceChannel
	
	Represents a Voice Channel within a Guild. 

	**0.9:** ``ChannelType.Voice``

Users
-----

.. function:: IUser
	
	Represents a basic user object. (``/users/:id``)

.. function:: ISelfUser
	
	Represents the current user object. (``/users/@me``)

	**0.9:** ``DiscordClient.CurrentUser``

.. function:: IDMUser
	
	Represents a User in a DM. Does not belong to an ``IGuild``.

.. function:: IGuildUser
	
	Represents a User in a Guild. Has a Nickname, VoiceChannel.

	**0.9:** ``User``

.. function:: IConnection
	
	Represents a Connection the CurrentUser has. (e.g. Twitch, Youtube Gaming)

Roles
-----

.. function:: IRole
	
	Represents a Role in an ``IGuild``.

	**0.9:** ``Role``

Messages
--------

.. function:: IMessage
	
	Represents a Message object.

	**0.9:** ``Message``

Usage
-----

Guilds (Servers)
----------------

Channels
--------

Users
-----