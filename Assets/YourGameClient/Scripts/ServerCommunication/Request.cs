using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using MagicOnion;
using MagicOnion.Client;
using CustomUnity;
using YourGameServer.Interface;
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
        static GrpcChannelx _globalChannel = null;
        public static GrpcChannelx GlobalChannel {
            get {
                _globalChannel ??= GrpcChannelx.ForAddress($"{rpcScheme}://{ServerAddr}:{serverRpcPort}");
                return _globalChannel;
            }
        }

        public const YourGameServer.Interface.DeviceType deviceType =
#if UNITY_IOS
            YourGameServer.Interface.DeviceType.IOS
#elif UNITY_ANDROID
            YourGameServer.Interface.DeviceType.Android
#elif UNITY_WEBGL
            YourGameServer.Interface.DeviceType.WebGL
#else
            YourGameServer.Interface.DeviceType.StandAlone
#endif
        ;

        const string PlayerId = nameof(Request) + ".playerid";
        const string PlayerCode = nameof(Request) + ".playercode";
        const string LastDeviceId = nameof(Request) + ".lastdeviceid";

        public static ContentType CurrentAcceptContentType = ContentType.MessagePack;
        public static ContentType CurrentRequestContentType = ContentType.MessagePack;
        public static ulong CurrentPlayerId = 0;
        public static string CurrentSecurityToken = null;
        public static DateTime CurrentSecurityTokenPeriod = DateTime.MaxValue;
        public static string LatestPlayerCode = PlayerPrefs.GetString(PlayerCode, string.Empty);

        /// <summary>
        /// return true when already logged in.
        /// </summary>
        public static bool IsLoggedIn => CurrentPlayerId > 0;

        public enum ContentType
        {
            MessagePack,
            JSON,
        }

        public static string ToHeaderString(this ContentType contentType)
        {
            return contentType switch {
                ContentType.MessagePack => "application/x-msgpack",
                ContentType.JSON => "application/json",
                _ => ""
            };
        }

        public static async UniTask<FormalPlayerAccount> GetPlayerAccount()
        {
            var client = MagicOnionClient.Create<IPlayerAccountService>(GlobalChannel, new IClientFilter[] { new AppendHeaderFilter() });
            var request = client.GetPlayerAccount();
            var result = await request;
            var status = request.GetStatus();
            if(status.StatusCode != Grpc.Core.StatusCode.OK) {
                Log.Error($"GetPlayerAccount : {status}");
            }
            return result;
        }

        public static async UniTask<IEnumerable<MaskedPlayerAccount>> GetPlayerAccounts(ulong[] ids)
        {
            var client = MagicOnionClient.Create<IPlayerAccountService>(GlobalChannel, new IClientFilter[] { new AppendHeaderFilter() });
            var request = client.GetPlayerAccounts(ids);
            var result = await request;
            var status = request.GetStatus();
            if(status.StatusCode != Grpc.Core.StatusCode.OK) {
                Log.Error($"GetPlayerAccount : {status}");
            }
            return result;
        }

        public static async UniTask<bool> SignUp()
        {
            var client = MagicOnionClient.Create<IAccountService>(GlobalChannel);
            Log.Info("SignUp : Call");
            var request = client.SignUp(new SignInRequest {
                DeviceType = deviceType,
                DeviceId = SystemInfo.deviceUniqueIdentifier
            });
            var result = await request;
            Log.Info($"SignUp : Call End {request.GetStatus()}");
            if(result != null) {
                Log.Info($"SignUp : {result}");
                CurrentPlayerId = result.Id;
                LatestPlayerCode = result.Code;
                CurrentSecurityToken = result.Token;
                CurrentSecurityTokenPeriod = result.Period;
                PlayerPrefs.Set(PlayerId, result.Id.ToString());
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
            if(!PlayerPrefs.HasKey(PlayerId)) return false;

            var playerId = ulong.Parse(PlayerPrefs.Get(PlayerId, "0"));
            var deviceId = PlayerPrefs.Get(LastDeviceId, SystemInfo.deviceUniqueIdentifier);
            bool deviceChanged = SystemInfo.deviceUniqueIdentifier != deviceId;
            // If valid newDeviceId call failed, it may means already accepted device change on server, try normal login with newDeviceId = null again.
            for(var newDeviceId = deviceChanged ? SystemInfo.deviceUniqueIdentifier : null; deviceId != null; deviceId = newDeviceId, newDeviceId = null) {
                var client = MagicOnionClient.Create<IAccountService>(GlobalChannel);
                Log.Info("LogIn : Call");
                var request = client.LogIn(new LogInRequest {
                    Id = playerId,
                    DeviceType = deviceType,
                    DeviceId = deviceId,
                    NewDeviceId = newDeviceId
                });
                var result = await request;
                Log.Info($"LogIn : End {request.GetStatus()}");
                if(result != null) {
                    Log.Info($"LogIn : {result}");
                    CurrentPlayerId = playerId;
                    bool needs_save = false;
                    if(LatestPlayerCode != result.Code) {
                        PlayerPrefs.Set(PlayerCode, result.Code);
                        needs_save = true;
                    }
                    if(deviceChanged) {
                        PlayerPrefs.Set(LastDeviceId, SystemInfo.deviceUniqueIdentifier);
                        needs_save = true;
                    }
                    if(needs_save) PlayerPrefs.Save();
                    LatestPlayerCode = result.Code;
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
            var client = MagicOnionClient.Create<IAccountService>(GlobalChannel, new IClientFilter[] { new AppendHeaderFilter() });
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
            var client = MagicOnionClient.Create<IAccountService>(GlobalChannel, new IClientFilter[] { new AppendHeaderFilter() });
            var request = client.LogOut();
            await request;
            CurrentPlayerId = 0;
            CurrentSecurityToken = null;
            CurrentSecurityTokenPeriod = DateTime.MaxValue;
            KeepConnect.Instance.enabled = false;
            Log.Info($"LogOut {request.GetStatus()}");
        }
    }

    public class AppendHeaderFilter : IClientFilter
    {
        public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
        {
            // add the common header(like authentication).
            var header = context.CallOptions.Headers;
            header.Add("Authorization", $"Bearer {Request.CurrentSecurityToken}");

            return await next(context);
        }
    }
}
