using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using MagicOnion;
using MagicOnion.Client;
using CustomUnity;
using YourGameServer.Game.Interface;
using PlayerPrefs = CustomUnity.PlayerPrefs;

namespace YourGameClient
{
    public static class Request
    {
        static string serverAddr
#if UNITY_IOS || UNITY_ANDROID && !UNITY_EDITOR
            = "192.168.11.15";
#else
            = "localhost";
#endif
        public static string ServerAddr {
            get => serverAddr;
            set {
                if(_globalChannel != null) {
                    _globalChannel.Dispose();
                    _globalChannel = null;
                }
                serverAddr = value;
            }
        }
        public const int serverRpcPort = 7143;
        public const string rpcScheme = "https";
        public static string ServerURL => $"{rpcScheme}://{ServerAddr}:{serverRpcPort}";

        static GrpcChannelx _globalChannel = null;
        public static GrpcChannelx GlobalChannel {
            get {
                _globalChannel ??= GrpcChannelx.ForAddress(ServerURL);
                return _globalChannel;
            }
        }

        public const YourGameServer.Game.Interface.DeviceType deviceType =
#if UNITY_IOS
            YourGameServer.Game.Interface.DeviceType.IOS
#elif UNITY_ANDROID
            YourGameServer.Game.Interface.DeviceType.Android
#elif UNITY_WEBGL
            YourGameServer.Game.Interface.DeviceType.WebGL
#else
            YourGameServer.Game.Interface.DeviceType.StandAlone
#endif
        ;

        const string PlayerCode = nameof(YourGameClient) + "." + nameof(Request) + ".playercode";
        const string LastDeviceId = nameof(YourGameClient) + "." + nameof(Request) + ".lastdeviceid";

        public static ContentType CurrentAcceptContentType = ContentType.MessagePack;
        public static ContentType CurrentRequestContentType = ContentType.MessagePack;
        public static string CurrentSecurityToken { get; private set; } = null;
        public static DateTime CurrentSecurityTokenPeriod { get; private set; } = DateTime.MaxValue;
        public static string CurrentPlayerCode { get; private set; } = PlayerPrefs.GetString(PlayerCode, string.Empty);

        /// <summary>
        /// return true when already logged in.
        /// </summary>
        public static bool IsLoggedIn => !string.IsNullOrEmpty(CurrentSecurityToken);

        public enum ContentType
        {
            MessagePack,
            JSON,
        }

        public static async UniTask ShutdownAsync()
        {
            if(_globalChannel != null) {
                var task = _globalChannel.ShutdownAsync();
                _globalChannel = null;
                await task;
            }
        }

        public static async UniTask<bool> SignUp()
        {
            var client = CreateAccountClient();
            Log.Info($"SignUp : Call DeviceType = {deviceType}, DeviceId = {SystemInfo.deviceUniqueIdentifier}");
            var request = client.SignUp(new() {
                DeviceType = deviceType,
                DeviceId = SystemInfo.deviceUniqueIdentifier
            });
            var result = await request;
            Log.Info($"SignUp : Call End {request.GetStatus()}");
            if(result != null) {
                Log.Info($"SignUp : {result}");
                CurrentPlayerCode = result.Code;
                CurrentSecurityToken = result.Token;
                CurrentSecurityTokenPeriod = result.Period;
                PlayerPrefs.Set(PlayerCode, result.Code);
                PlayerPrefs.Set(LastDeviceId, SystemInfo.deviceUniqueIdentifier);
                PlayerPrefs.Save();
                KeepConnect.Instance.enabled = true;
                return true;
            }
            else {
                Log.Error($"NewAccount : failed");
            }
            return false;
        }

        public static async UniTask<bool> LogIn()
        {
            if(!PlayerPrefs.HasKey(PlayerCode)) return false;

            var playerCode = PlayerPrefs.Get(PlayerCode, string.Empty);
            var deviceId = PlayerPrefs.Get(LastDeviceId, SystemInfo.deviceUniqueIdentifier);
            bool deviceChanged = SystemInfo.deviceUniqueIdentifier != deviceId;
            // If valid newDeviceId call failed, it may means already accepted device change on server, try normal login with newDeviceId = null again.
            for(var newDeviceId = deviceChanged ? SystemInfo.deviceUniqueIdentifier : null; deviceId != null; deviceId = newDeviceId, newDeviceId = null) {
                var client = CreateAccountClient();
                Log.Info($"LogIn : Call Code = {playerCode}, DeviceType = {deviceType}, DeviceId = {deviceId}, NewDeviceId = {newDeviceId}");
                var request = client.LogIn(new() {
                    Code = playerCode,
                    DeviceType = deviceType,
                    DeviceId = deviceId,
                    NewDeviceId = newDeviceId
                });
                var result = await request;
                Log.Info($"LogIn : End {request.GetStatus()}");
                if(result != null) {
                    Log.Info($"LogIn : {result}");
                    bool needs_save = false;
                    if(deviceChanged) {
                        PlayerPrefs.Set(LastDeviceId, SystemInfo.deviceUniqueIdentifier);
                        needs_save = true;
                    }
                    if(needs_save) PlayerPrefs.Save();
                    CurrentSecurityToken = result.Token;
                    CurrentSecurityTokenPeriod = result.Period;
                    KeepConnect.Instance.enabled = true;
                    return true;
                }
            }
            return false;
        }

        public static async UniTask<bool> RenewToken()
        {
            var client = CreateAccountClient();
            var result = await client.RenewToken();
            if(result != null) {
                Log.Info($"RenewToken : {result}");
                CurrentSecurityToken = result.Token;
                CurrentSecurityTokenPeriod = result.Period;
                return true;
            }
            return false;
        }

        public static async UniTask LogOut()
        {
            var client = CreateAccountClient();
            var request = client.LogOut();
            await request;
            CurrentSecurityToken = null;
            CurrentSecurityTokenPeriod = DateTime.MaxValue;
            KeepConnect.Instance.enabled = false;
            Log.Info($"LogOut {request.GetStatus()}");
        }

        public static async UniTask<FormalPlayerAccount> GetPlayerAccount()
        {
            var client = CreatePlayerAccountClient();
            var request = client.GetPlayerAccount();
            var result = await request;
            var status = request.GetStatus();
            if(status.StatusCode != Grpc.Core.StatusCode.OK) {
                Log.Error($"GetPlayerAccount : {status}");
            }
            return result;
        }

        public static async UniTask<IEnumerable<MaskedPlayerAccount>> GetPlayerAccounts(string[] codes)
        {
            var client = CreatePlayerAccountClient();
            var request = client.GetPlayerAccounts(codes);
            var result = await request;
            var status = request.GetStatus();
            if(status.StatusCode != Grpc.Core.StatusCode.OK) {
                Log.Error($"GetPlayerAccount : {status}");
            }
            return result;
        }

        public static async UniTask<IEnumerable<MaskedPlayerAccount>> FindPlayerAccounts(int maxCount)
        {
            var client = CreatePlayerAccountClient();
            var request = client.FindPlayerAccounts(maxCount);
            var result = await request;
            var status = request.GetStatus();
            if(status.StatusCode != Grpc.Core.StatusCode.OK) {
                Log.Error($"FindPlayerAccounts : {status}");
            }
            return result;
        }

        static IAccountService CreateAccountClient()
            => CurrentSecurityToken != null
            ? MagicOnionClient.Create<IAccountService>(GlobalChannel, new[] { new AppendHeaderFilter() })
            : MagicOnionClient.Create<IAccountService>(GlobalChannel);

        static IPlayerAccountService CreatePlayerAccountClient()
            => MagicOnionClient.Create<IPlayerAccountService>(GlobalChannel, new[] { new AppendHeaderFilter() });

        class AppendHeaderFilter : IClientFilter
        {
            public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
            {
                // add the common header(like authentication).
                var header = context.CallOptions.Headers;
                header.Add("Authorization", $"Bearer {CurrentSecurityToken}");
                return await next(context);
            }
        }
    }
}
