Events
======

|updated|  

|incomplete|  


Usage
-----
Messages from Discord are exposed via events, and follow a pattern of ``Func<(event params), Task>``. 

To hook into Events, you must be using the ``DiscordSocketClient``, which provides WebSocket capabilities, necessary for receiving events.

..note::
    
    The Gateway will wait for all registered handlers of an event to finish before continuing with 
    raising the next event. As a result of this, it is reccomended that if  you need to perform 
    any heavy work in an event handler, it is done on its own thread or Task.

Connection State
----------------

Connection Events will be raised when the Connection State of your client changes.

``DiscordSocketClient.Connected`` and ``Disconnected`` are raised when the Gateway Socket connects or disconnects, respectively.

.. warning::
    You should not use DiscordClient.Connected to run code when your client first connects to Discord.
    The client has not received the READY event yet, and will have an incomplete or empty cache.

``DiscordSocketClient.Ready`` is raised when the ``READY`` packet is received and parsed from Discord.

.. note::
    The ``DiscordSocketClient.ConnectAsync`` method will not return until the Ready event has been fired. By default, it will also not fire until all of the bot's guilds have been received (in the case of bot accounts). This means it is safe to run code directly after awaiting the ConnectAsync method.

    
Messages
--------

- MessageReceived is raised when a message is received, and contains only an ``IMessage`` object as its parameter.
- MessageUpdated is raised when a message is edited, and contains an ``Optional<IMessage>`` and an ``IMessage``, representing the before/after states of the message. The internal message cache may not always have the before message cached, so the first message may not have a value in some cases.
- MessageDeleted is raised when a message is deleted, and contains a ``ulong`` and an ``Optional<IMessage>``, representing the ID of the message, and in some cases the message object. The message object may not always be complete, so it will not always have a value.

Example of MessageReceived:

.. code-block:: c#

    // (Preface: Echo Bots are discouraged, make sure your bot is not running in a public server if you use them)

    // Hook into the MessageReceived event using a Lambda
    _client.MessageReceived += async (message) => {
            if (e.Message.Author.Id != (await _client.GetCurrentUserAsync()).Id)
                await msg.Channel.SendMessageAsync(message.Text);
    };

Users
-----

There are several user events:

- UserBanned: A user has been banned from a guild; raised with an ``IUser`` and ``IGuild``.
- UserUnbanned: A user was unbanned; raised with an ``IUser`` and ``IGuild``.
- UserJoined: A user joins a guild; raised with an ``IGuildUser``.
- UserLeft: A user left (or was kicked from) a guild; raised with an ``IGuildUser``.
- UserIsTyping: A user in a channel starts typing; raised with an ``IUser`` and ``IChannel``.
- UserUpdated: A user object was updated (role/permission change); raised with an ``IGuildUser`` and ``IGuildUser``, representing the before/after states of the user.
- UserPresenceUpdated: A user's presence was updated; raised with an ``IGuildUser``, and two ``IPresence``, representing the user, and the before/after presences of the user.
- UserVoiceStateUpdated: A user's voice state was updated; raised with an ``IGuildUser``, and two ``IVoiceState``, representing the user, and the before/after voice states of the user.

Examples:

.. code-block:: c#

    // Register a Hook into the UserBanned event using a Lambda
    _client.UserBanned += async (user, guild) => {
        // Get the #mod_log channel on the server by ID, and cast it to an IMessageChannel.
        var logChannel = await guild.GetChannelAsync(173201159761297408) as IMessageChannel;
        // Send a message to the server's log channel, stating that a user was banned.
        await logChannel.SendMessageAsync($"User Banned: {User.Username}");
    };

    // Register a Hook into the UserVoiceStateUpdated event using a Lambda
    _client.UserVoiceStateUpdated += async (user, before, after) => {
        // The user just joined a voice channel, return.
        if (before.VoiceChannel == null) return;

        // See if they changed Voice channels
        if (before.VoiceChannel == after.VoiceChannel) return;

        await logChannel.SendMessage($"User {user.Username} changed voice channels!");
    };
