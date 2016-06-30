Guild Management
================

|updated|  

|incomplete|

Discord.Net will allow you to manage most settings of a Guild.

Usage
-----

You can create Channels, Invites, and Roles on a guild using the CreateTextChannelAsync, CreateVoiceChannelAsync, CreateInviteAsync, and CreateRoleAsync functions.

You may also modify a Guild using the ModifyAsync method.

.. code-block:: c#

    // Create a Text Channel and retrieve the ITextChannel
    ITextChannel _textChannel = await _guild.CreateTextChannelAsync("announcements");
    // Create a Voice Channel and retreieve the IVoiceChannel
    IVoiceChannel _voiceChannel = await _guild.CreateVoiceChannelAsync("music")

    // Create an Invite and retrieve the IInvite
    IInvite _invite = await _guild.CreateInviteAsync(maxAge: null, maxUses: 25, tempMembership: false, withXkcd: false);

    // Create a Role and retrieve the Role object
    IRole _role = await _guild.CreateRoleAsync(name: "Music Bots", 
        permissions: new GuildPermissions(connect: true), 
        color: new Color(180, 15, 140), 
        isHoisted: false);

    // Edit a Guild
    var _ioStream = new System.IO.StreamReader("clock-0500-1952.png").BaseStream;
    var _region = (await _client.GetVoiceRegionsAsync()).FirstOrDefault(v => v.Id == "us-east");
    await _guild.ModifyAsync(x => {
        x.Name: "19:52 | UTC-05:00", 
        x.Region: _region, 
        x.Icon: _ioStream, 
    });

    // Prune Users
    await _guild.PruneUsersAsync(30);