using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using YourGameServer.Game.Interface;
using CustomUnity;

namespace YourGameClient
{
    public class ChatClient : IChatHubReceiver
    {
        public bool HasJoined => _hasJoined;
        public Guid CurrentContextId { get; private set; }

        public Action<bool> onDisconnectedByServer;

        public Action<ChatMessage> onRecievedMessage;

        readonly CancellationTokenSource _shutdownCancellation = new();
        ChannelBase _channel;
        IChatHub _streamingClient;
        bool _hasJoined;
        bool _isSelfDisConnected;

        ChatJoinRequest _joinRequest;

        public async UniTask StartAsync(string roomName, string userName)
        {
            _joinRequest = new() { RoomName = roomName, UserName = userName };
            await InitializeClientAsync();
            await JoinAsync();
            _hasJoined = true;
        }

        public async UniTask ShutdownAsync()
        {
            // Clean up Hub and channel
            _shutdownCancellation.Cancel();

            if(_streamingClient != null) await _streamingClient.DisposeAsync();
            if(_channel != null) await _channel.ShutdownAsync();
        }

        async UniTask InitializeClientAsync()
        {
            // Initialize the Hub
            // NOTE: If you want to use SSL/TLS connection, see InitialSettings.OnRuntimeInitialize method.
            _channel = GrpcChannelx.ForAddress(Request.ServerURL);

            while(!_shutdownCancellation.IsCancellationRequested) {
                try {
                    Log.Info($"Connecting to the server...");
                    _streamingClient = await StreamingHubClient.ConnectAsync<IChatHub, IChatHubReceiver>(
                        _channel, this, option: new(headers: new() { new("Authorization", Request.CurrentSecurityToken) }),
                        cancellationToken: _shutdownCancellation.Token
                    );
                    RegisterDisconnectEvent(_streamingClient);
                    Log.Info($"Connection is established.");
                    break;
                }
                catch(Exception e) {
                    Log.Exception(e);
                }

                Log.Info($"Failed to connect to the server. Retry after 5 seconds...");
                await UniTask.Delay(5 * 1000);
            }
        }

        async void RegisterDisconnectEvent(IChatHub streamingClient)
        {
            try {
                // you can wait disconnected event
                await streamingClient.WaitForDisconnect();
            }
            catch(Exception e) {
                Log.Exception(e);
            }
            finally {
                // try-to-reconnect? logging event? close? etc...
                Log.Info($"disconnected from the server.");

                onDisconnectedByServer?.Invoke(_isSelfDisConnected);

                if(_isSelfDisConnected) {
                    // there is no particular meaning
                    await UniTask.Delay(2000);
                    // reconnect
                    await ReconnectServerAsync();
                }
            }
        }

        async UniTask ReconnectServerAsync()
        {
            Log.Info($"Reconnecting to the server...");
            _streamingClient = await StreamingHubClient.ConnectAsync<IChatHub, IChatHubReceiver>(
                _channel, this, option: new(headers: new() { new("Authorization", Request.CurrentSecurityToken) })
            );
            RegisterDisconnectEvent(_streamingClient);
            Log.Info("Reconnected.");

            _isSelfDisConnected = false;
        }

        #region Client -> Server (Streaming)
        public async UniTask JoinAsync()
        {
            if(!_hasJoined) {
                CurrentContextId = await _streamingClient.JoinAsync(_joinRequest);
                _hasJoined = true;
            }
        }

        public async UniTask LeaveAsync()
        {
            if(_hasJoined) {
                await _streamingClient.LeaveAsync();
                _hasJoined = false;
            }
        }

        public async UniTask SendMessageAsync(string message)
        {
            if(_hasJoined) {
                await _streamingClient.SendMessageAsync(message);
            }
        }
        #endregion

        #region Server -> Client (Streaming)
        public void OnJoin(ChatMember member)
        {
            Log.Info($"{member.UserName} entered the room.".Grey());
        }

        public void OnLeave(ChatMember member)
        {
            Log.Info($"{member.UserName} left the room.".Grey());
        }

        public void OnRecievedMessage(ChatMessage message)
        {
            onRecievedMessage?.Invoke(message);
            Log.Info($"{message.Member.UserName}:{message.Message}".Grey());
        }
        #endregion
    }
}
