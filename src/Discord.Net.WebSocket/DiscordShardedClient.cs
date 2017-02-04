using Discord.API;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Discord.WebSocket
{
    public partial class DiscordShardedClient : BaseDiscordClient, IDiscordClient
    {
        private readonly DiscordSocketConfig _baseConfig;
        private readonly SemaphoreSlim _connectionGroupLock;
        private int[] _shardIds;
        private Dictionary<int, int> _shardIdsToIndex;
        private DiscordSocketClient[] _shards;
        private int _totalShards;
        private bool _automaticShards;
        
        /// <summary> Gets the estimated round-trip latency, in milliseconds, to the gateway server. </summary>
        public int Latency => GetLatency();
        public UserStatus Status => _shards[0].Status;
        public Game? Game => _shards[0].Game;

        internal new DiscordSocketApiClient ApiClient => base.ApiClient as DiscordSocketApiClient;
        public new SocketSelfUser CurrentUser { get { return base.CurrentUser as SocketSelfUser; } private set { base.CurrentUser = value; } }
        public IReadOnlyCollection<SocketGuild> Guilds => GetGuilds().ToReadOnlyCollection(() => GetGuildCount());
        public IReadOnlyCollection<ISocketPrivateChannel> PrivateChannels => GetPrivateChannels().ToReadOnlyCollection(() => GetPrivateChannelCount());
        public IReadOnlyCollection<DiscordSocketClient> Shards => _shards;
        public IReadOnlyCollection<RestVoiceRegion> VoiceRegions => _shards[0].VoiceRegions;

        //my stuff
        //private uint _connectedCount = 0;
        //private uint _downloadedCount = 0;
        
        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordShardedClient() : this(null, new DiscordSocketConfig()) { }
        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordShardedClient(DiscordSocketConfig config) : this(null, config, CreateApiClient(config)) { }
        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordShardedClient(int[] ids) : this(ids, new DiscordSocketConfig()) { }
        /// <summary> Creates a new REST/WebSocket discord client. </summary>
        public DiscordShardedClient(int[] ids, DiscordSocketConfig config) : this(ids, config, CreateApiClient(config)) { }
        private DiscordShardedClient(int[] ids, DiscordSocketConfig config, API.DiscordSocketApiClient client)
            : base(config, client)
        {
            if (config.ShardId != null)
                throw new ArgumentException($"{nameof(config.ShardId)} must not be set.");
            if (ids != null && config.TotalShards == null)
                throw new ArgumentException($"Custom ids are not supported when {nameof(config.TotalShards)} is not specified.");

            _shardIdsToIndex = new Dictionary<int, int>();
            config.DisplayInitialLog = false;
            _baseConfig = config;
            _connectionGroupLock = new SemaphoreSlim(1, 1);

            if (config.TotalShards == null)
                _automaticShards = true;
            else
            {
                _totalShards = config.TotalShards.Value;
                _shardIds = ids ?? Enumerable.Range(0, _totalShards).ToArray();
                _shards = new DiscordSocketClient[_shardIds.Length];
                for (int i = 0; i < _shardIds.Length; i++)
                {
                    _shardIdsToIndex.Add(_shardIds[i], i);
                    var newConfig = config.Clone();
                    newConfig.ShardId = _shardIds[i];
                    _shards[i] = new DiscordSocketClient(newConfig, _connectionGroupLock, i != 0 ? _shards[0] : null);
                    RegisterEvents(_shards[i]);
                }
            }
        }
        private static API.DiscordSocketApiClient CreateApiClient(DiscordSocketConfig config)
            => new API.DiscordSocketApiClient(config.RestClientProvider, DiscordRestConfig.UserAgent, config.WebSocketProvider);

        protected override async Task OnLoginAsync(TokenType tokenType, string token)
        {
            if (_automaticShards)
            {
                var response = await ApiClient.GetBotGatewayAsync().ConfigureAwait(false);
                _shardIds = Enumerable.Range(0, response.Shards).ToArray();
                _totalShards = _shardIds.Length;
                _shards = new DiscordSocketClient[_shardIds.Length];
                for (int i = 0; i < _shardIds.Length; i++)
                {
                    _shardIdsToIndex.Add(_shardIds[i], i);
                    var newConfig = _baseConfig.Clone();
                    newConfig.ShardId = _shardIds[i];
                    newConfig.TotalShards = _totalShards;
                    _shards[i] = new DiscordSocketClient(newConfig, _connectionGroupLock, i != 0 ? _shards[0] : null);
                    RegisterEvents(_shards[i]);
                }
            }

            //Assume threadsafe: already in a connection lock
            for (int i = 0; i < _shards.Length; i++)
                await _shards[i].LoginAsync(tokenType, token, false);
        }
        protected override async Task OnLogoutAsync()
        {
            //Assume threadsafe: already in a connection lock
            for (int i = 0; i < _shards.Length; i++)
                await _shards[i].LogoutAsync();

            CurrentUser = null;
            if (_automaticShards)
            {
                _shardIds = new int[0];
                _shardIdsToIndex.Clear();
                _totalShards = 0;
                _shards = null;
            }
        }

        /// <inheritdoc />
        public async Task ConnectAsync()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await ConnectInternalAsync().ConfigureAwait(false);
            }
            catch
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
                throw;
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ConnectInternalAsync()
        {
            await Task.WhenAll(
                _shards.Select(x => x.ConnectAsync())
            ).ConfigureAwait(false);

            CurrentUser = _shards[0].CurrentUser;
        }
        /// <inheritdoc />
        public async Task DisconnectAsync()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task DisconnectInternalAsync()
        {
            for (int i = 0; i < _shards.Length; i++)
                await _shards[i].DisconnectAsync();
        }

        public DiscordSocketClient GetShard(int id)
        {
            if (_shardIdsToIndex.TryGetValue(id, out id))
                return _shards[id];
            return null;
        }
        public int GetShardIdFor(ulong guildId)
            => (int)((guildId >> 22) % (uint)_totalShards);
        public int GetShardIdFor(IGuild guild)
            => GetShardIdFor(guild.Id);
        private DiscordSocketClient GetShardFor(ulong guildId)
            => GetShard(GetShardIdFor(guildId));
        public DiscordSocketClient GetShardFor(IGuild guild)
            => GetShardFor(guild.Id);

        /// <inheritdoc />
        public async Task<RestApplication> GetApplicationInfoAsync()
            => await _shards[0].GetApplicationInfoAsync().ConfigureAwait(false);

        /// <inheritdoc />
        public SocketGuild GetGuild(ulong id) => GetShardFor(id).GetGuild(id);
        /// <inheritdoc />
        public Task<RestGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null)
            => ClientHelper.CreateGuildAsync(this, name, region, jpegIcon);

        /// <inheritdoc />
        public SocketChannel GetChannel(ulong id)
        {
            for (int i = 0; i < _shards.Length; i++)
            {
                var channel = _shards[i].GetChannel(id);
                if (channel != null)
                    return channel;
            }
            return null;
        }

        public Task<IDMChannel> GetDMChannelAsync(ulong channelId) =>
            _shards[0].GetDMChannelAsync(channelId);

        private IEnumerable<ISocketPrivateChannel> GetPrivateChannels()
        {
            for (int i = 0; i < _shards.Length; i++)
            {
                foreach (var channel in _shards[i].PrivateChannels)
                    yield return channel;
            }
        }
        private int GetPrivateChannelCount()
        {
            int result = 0;
            for (int i = 0; i < _shards.Length; i++)
                result += _shards[i].PrivateChannels.Count;
            return result;
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<RestConnection>> GetConnectionsAsync()
            => ClientHelper.GetConnectionsAsync(this);

        public IEnumerable<SocketGuild> GetGuilds()
        {
            for (int i = 0; i < _shards.Length; i++)
            {
                foreach (var guild in _shards[i].Guilds)
                    yield return guild;
            }
        }

        public int GetGuildCount()
        {
            int result = 0;
            for (int i = 0; i < _shards.Length; i++)
                result += _shards[i].Guilds.Count;
            return result;
        }

        /// <inheritdoc />
        public Task<RestInvite> GetInviteAsync(string inviteId)
            => ClientHelper.GetInviteAsync(this, inviteId);

        /// <inheritdoc />
        public SocketUser GetUser(ulong id)
        {
            for (int i = 0; i < _shards.Length; i++)
            {
                var user = _shards[i].GetUser(id);
                if (user != null)
                    return user;
            }
            return null;
        }
        /// <inheritdoc />
        public SocketUser GetUser(string username, string discriminator)
        {
            for (int i = 0; i < _shards.Length; i++)
            {
                var user = _shards[i].GetUser(username, discriminator);
                if (user != null)
                    return user;
            }
            return null;
        }

        /// <inheritdoc />
        public RestVoiceRegion GetVoiceRegion(string id)
            => _shards[0].GetVoiceRegion(id);

        /// <summary> Downloads the users list for the provided guilds, if they don't have a complete list. </summary>
        public async Task DownloadUsersAsync(IEnumerable<SocketGuild> guilds)
        {
            for (int i = 0; i < _shards.Length; i++)
            {
                int id = _shardIds[i];
                var arr = guilds.Where(x => GetShardIdFor(x) == id).ToArray();
                if (arr.Length > 0)
                    await _shards[i].DownloadUsersAsync(arr);
            }
        }

        private int GetLatency()
        {
            int total = 0;
            for (int i = 0; i < _shards.Length; i++)
                total += _shards[i].Latency;
            return (int)Math.Round(total / (double)_shards.Length);
        }

        public async Task SetStatusAsync(UserStatus status)
        {
            for (int i = 0; i < _shards.Length; i++)
                await _shards[i].SetStatusAsync(status).ConfigureAwait(false);
        }
        public async Task SetGameAsync(string name, string streamUrl = null, StreamType streamType = StreamType.NotStreaming)
        {
            for (int i = 0; i < _shards.Length; i++)
            {
                try
                {
                    await _shards[i].SetGameAsync(name, streamUrl, streamType).ConfigureAwait(false);
                }
                catch (Exception ex) {
                    await _logEvent?.InvokeAsync(new LogMessage(LogSeverity.Warning, "DiscordShardedClient", "Setting game failed on shard " + i, ex));
                }
            }
        }

        private void RegisterEvents(DiscordSocketClient client)
        {
            client.Log += (msg) => { _logEvent.InvokeAsync(msg); return Task.FromResult(0); };
            client.LoggedOut += () =>
            {
                var state = LoginState;
                if (state == LoginState.LoggedIn || state == LoginState.LoggingIn)
                {
                    //Should only happen if token is changed
                    var _ = LogoutAsync(); //Signal the logout, fire and forget
                }
                return Task.Delay(0);
            };

            client.ChannelCreated += (channel) => { _channelCreatedEvent.InvokeAsync(channel); return Task.FromResult(0); };
            client.ChannelDestroyed += (channel) => { _channelDestroyedEvent.InvokeAsync(channel); return Task.FromResult(0); };
            client.ChannelUpdated += (oldChannel, newChannel) => { _channelUpdatedEvent.InvokeAsync(oldChannel, newChannel); return Task.FromResult(0); };

            client.MessageReceived += (msg) =>
            {
                if (msg.Author == null || msg.Author.IsBot)
                    return Task.FromResult(0);
                _messageReceivedEvent.InvokeAsync(msg);
                return Task.FromResult(0);
            };
            client.MessageDeleted += (id, msg) => { _messageDeletedEvent.InvokeAsync(id, msg); return Task.FromResult(0); };
            client.MessageUpdated += (oldMsg, newMsg) => { _messageUpdatedEvent.InvokeAsync(oldMsg, newMsg); return Task.FromResult(0); };
            client.ReactionAdded += (id, msg, reaction) => { _reactionAddedEvent.InvokeAsync(id, msg, reaction); return Task.FromResult(0); };
            client.ReactionRemoved += (id, msg, reaction) => { _reactionRemovedEvent.InvokeAsync(id, msg, reaction); return Task.FromResult(0); };
            client.ReactionsCleared += (id, msg) => { _reactionsClearedEvent.InvokeAsync(id, msg); return Task.FromResult(0); };
            
            client.RoleCreated += (role) => { _roleCreatedEvent.InvokeAsync(role); return Task.FromResult(0); };
            client.RoleDeleted += (role) => { _roleDeletedEvent.InvokeAsync(role); return Task.FromResult(0); };
            client.RoleUpdated += (oldRole, newRole) => { _roleUpdatedEvent.InvokeAsync(oldRole, newRole); return Task.FromResult(0); };

            client.JoinedGuild += (guild) => { _joinedGuildEvent.InvokeAsync(guild); return Task.FromResult(0); };
            client.LeftGuild += (guild) => { _leftGuildEvent.InvokeAsync(guild); return Task.FromResult(0); };
            client.GuildAvailable += (guild) => { _guildAvailableEvent.InvokeAsync(guild); return Task.FromResult(0); };
            client.GuildUnavailable += (guild) => { _guildUnavailableEvent.InvokeAsync(guild); return Task.FromResult(0); };
            client.GuildMemberUpdated += (oldUser, newUser) => { _guildMemberUpdatedEvent.InvokeAsync(oldUser, newUser); return Task.FromResult(0); };
            client.GuildMembersDownloaded += (guild) => { _guildMembersDownloadedEvent.InvokeAsync(guild); return Task.FromResult(0); };
            client.GuildUpdated += (oldGuild, newGuild) => { _guildUpdatedEvent.InvokeAsync(oldGuild, newGuild); return Task.FromResult(0); };

            client.UserJoined += (user) => { _userJoinedEvent.InvokeAsync(user); return Task.FromResult(0); };
            client.UserLeft += (user) => { _userLeftEvent.InvokeAsync(user); return Task.FromResult(0); };
            client.UserBanned += (user, guild) => { _userBannedEvent.InvokeAsync(user, guild); return Task.FromResult(0); };
            client.UserUnbanned += (user, guild) => { _userUnbannedEvent.InvokeAsync(user, guild); return Task.FromResult(0); };
            client.UserUpdated += (oldUser, newUser) => { _userUpdatedEvent.InvokeAsync(oldUser, newUser); return Task.FromResult(0); };
            client.UserPresenceUpdated += (guild, user, oldPresence, newPresence) => { _userPresenceUpdatedEvent.InvokeAsync(guild, user, oldPresence, newPresence); return Task.FromResult(0); };
            client.UserVoiceStateUpdated += (user, oldVoiceState, newVoiceState) => { _userVoiceStateUpdatedEvent.InvokeAsync(user, oldVoiceState, newVoiceState); return Task.FromResult(0); };
            client.CurrentUserUpdated += (oldUser, newUser) => { _selfUpdatedEvent.InvokeAsync(oldUser, newUser); return Task.FromResult(0); };
            client.UserIsTyping += (oldUser, newUser) => { _userIsTypingEvent.InvokeAsync(oldUser, newUser); return Task.FromResult(0); };
            client.RecipientAdded += (user) => { _recipientAddedEvent.InvokeAsync(user); return Task.FromResult(0); };
            client.RecipientAdded += (user) => { _recipientRemovedEvent.InvokeAsync(user); return Task.FromResult(0); };
        }

        //IDiscordClient
        Task IDiscordClient.ConnectAsync()
            => ConnectAsync();

        async Task<IApplication> IDiscordClient.GetApplicationInfoAsync()
            => await GetApplicationInfoAsync().ConfigureAwait(false);

        Task<IChannel> IDiscordClient.GetChannelAsync(ulong id, CacheMode mode)
            => Task.FromResult<IChannel>(GetChannel(id));
        Task<IReadOnlyCollection<IPrivateChannel>> IDiscordClient.GetPrivateChannelsAsync(CacheMode mode)
            => Task.FromResult<IReadOnlyCollection<IPrivateChannel>>(PrivateChannels);

        async Task<IReadOnlyCollection<IConnection>> IDiscordClient.GetConnectionsAsync()
            => await GetConnectionsAsync().ConfigureAwait(false);

        async Task<IInvite> IDiscordClient.GetInviteAsync(string inviteId)
            => await GetInviteAsync(inviteId).ConfigureAwait(false);

        Task<IGuild> IDiscordClient.GetGuildAsync(ulong id, CacheMode mode)
            => Task.FromResult<IGuild>(GetGuild(id));
        Task<IReadOnlyCollection<IGuild>> IDiscordClient.GetGuildsAsync(CacheMode mode)
            => Task.FromResult<IReadOnlyCollection<IGuild>>(Guilds);
        async Task<IGuild> IDiscordClient.CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon)
            => await CreateGuildAsync(name, region, jpegIcon).ConfigureAwait(false);

        Task<IUser> IDiscordClient.GetUserAsync(ulong id, CacheMode mode)
            => Task.FromResult<IUser>(GetUser(id));
        Task<IUser> IDiscordClient.GetUserAsync(string username, string discriminator)
            => Task.FromResult<IUser>(GetUser(username, discriminator));

        Task<IReadOnlyCollection<IVoiceRegion>> IDiscordClient.GetVoiceRegionsAsync()
            => Task.FromResult<IReadOnlyCollection<IVoiceRegion>>(VoiceRegions);
        Task<IVoiceRegion> IDiscordClient.GetVoiceRegionAsync(string id)
            => Task.FromResult<IVoiceRegion>(GetVoiceRegion(id));
    }
}
