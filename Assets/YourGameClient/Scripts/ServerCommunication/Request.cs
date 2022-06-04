#if UNITY_IOS || UNITY_ANDROID && DEVELOPMENT_BUILD && !UNITY_EDITOR
#else
# define USE_API_TSL
# define USE_RPC_TSL
#endif
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using MagicOnion;
using MagicOnion.Client;
using MessagePack;
using CustomUnity;
using YourGameServer.Models;
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
                apiRootUrl = null;
                if(globalChannel != null) {
                    globalChannel.Dispose();
                    globalChannel = null;
                }
                serverAddr = value;
            }
        }
        static string apiRootUrl = null;
#if USE_API_TSL
        public const int serverPort = 7142;
        public const string scheme = "https";
#else
        public const int serverPort = 5018;
        public const string scheme = "http";
#endif
        public static string ApiRootUrl {
            get {
                if(apiRootUrl is null) apiRootUrl = $"{scheme}://{ServerAddr}:{serverPort}/api";
                return apiRootUrl;
            }
        }
#if USE_RPC_TSL
        public const int serverRpcPort = 7143;
        public const string rpcScheme = "https";
#else
        public const int serverRpcPort = 5019;
        public const string rpcScheme = "http";
#endif
        static GrpcChannelx globalChannel = null;
        public static GrpcChannelx GlobalChannel {
            get {
                if(globalChannel == null) {
                    globalChannel = GrpcChannelx.ForAddress($"{rpcScheme}://{ServerAddr}:{serverRpcPort}");
                }
                return globalChannel;
            }
        }

        public const YourGameServer.Models.DeviceType deviceType =
#if UNITY_IOS
            YourGameServer.Models.DeviceType.IOS
#elif UNITY_ANDROID
            YourGameServer.Models.DeviceType.Android
#elif UNITY_WEBGL
            YourGameServer.Models.DeviceType.WebGL
#else
            YourGameServer.Models.DeviceType.StandAlone
#endif
        ;

        const string kPlayerId = nameof(Request) + ".playerid";
        const string kPlayerCode = nameof(Request) + ".playercode";
        const string kLastDeviceId = nameof(Request) + ".lastdeviceid";

        public static ContentType CurrentAcceptContentType = ContentType.MessagePack;
        public static ContentType CurrentRequestContentType = ContentType.MessagePack;
        public static ulong CurrentPlayerId = 0;
        public static string CurrentPlayerCode = PlayerPrefs.GetString(kPlayerCode, string.Empty);
        public static string CurrentSecurityToken = null;
        public static DateTime CurrentSecurityTokenPeriod = DateTime.MaxValue;

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

        internal static UnityWebRequest PostRequest<T>(string uri, T content)
        {
            if(CurrentRequestContentType == ContentType.MessagePack) {
                return WebRequest.Post(uri, MessagePackSerializer.Serialize(content), "application/x-msgpack");
            }
            return WebRequest.PostJson(uri, Newtonsoft.Json.JsonConvert.SerializeObject(content));
        }

        public static async UniTask<FormalPlayerAccount> GetPlayerAccount()
        {
            using var request = UnityWebRequest.Get($"{ApiRootUrl}/PlayerAccounts/{CurrentPlayerId}");
            request.SetRequestHeader("Accept", CurrentAcceptContentType.ToHeaderString());
            request.SetRequestHeader("Authorization", $"Bearer {CurrentSecurityToken}");
            Log.Info($"CurrentSecurityToken : {CurrentSecurityToken}");

            await request.SendWebRequest();
            if(request.error == null) {
                Log.Info($"Content-Type : {request.GetResponseHeader("Content-Type")}");
                if(request.GetResponseHeader("Content-Type").Contains("application/x-msgpack")) {
                    return MessagePackSerializer.Deserialize<FormalPlayerAccount>(request.downloadHandler.data);
                }
                else if(request.GetResponseHeader("Content-Type").Contains("application/json")) {
                    Log.Info($"source json : {request.downloadHandler.text}");
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<FormalPlayerAccount>(request.downloadHandler.text);
                }
                else {
                    Log.Error($"GetPlayerAccount : Unknown Format");
                    return null;
                }
            }
            else {
                Log.Error($"GetPlayerAccount : {request.error}");
            }
            return null;
        }

        public static async UniTask<IEnumerable<MaskedPlayerAccount>> GetPlayerAccounts(ulong[] ids)
        {
            using var request = UnityWebRequest.Get($"{ApiRootUrl}/PlayerAccounts?{string.Join('&', ids.Select(ids => "id=" + ids.ToString()))}");
            request.SetRequestHeader("Accept", CurrentAcceptContentType.ToHeaderString());
            request.SetRequestHeader("Authorization", $"Bearer {CurrentSecurityToken}");
            Log.Info($"CurrentSecurityToken : {CurrentSecurityToken}");

            await request.SendWebRequest();
            if(request.error == null) {
                Log.Info($"Content-Type : {request.GetResponseHeader("Content-Type")}");
                if(request.GetResponseHeader("Content-Type").Contains("application/x-msgpack")) {
                    return MessagePackSerializer.Deserialize<IEnumerable<MaskedPlayerAccount>>(request.downloadHandler.data);
                }
                else if(request.GetResponseHeader("Content-Type").Contains("application/json")) {
                    Log.Info($"source json : {request.downloadHandler.text}");
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<MaskedPlayerAccount>>(request.downloadHandler.text);
                }
                else {
                    Log.Error($"GetPlayerAccount : Unknown Format");
                    return null;
                }
            }
            else {
                Log.Error($"GetPlayerAccount : {request.error}");
            }
            return null;
        }

        public static async UniTask<bool> SignUp()
        {
            var client = MagicOnionClient.Create<IAccountService>(GlobalChannel);
            Log.Info("SignUp : Call");
            var result = await client.SignUp(new SignInRequest {
                DeviceType = deviceType,
                DeviceId = SystemInfo.deviceUniqueIdentifier
            });
            Log.Info("SignUp : Call End");
            if(result != null) {
                Log.Info($"SignUp : {result}");
                CurrentPlayerId = result.Id;
                CurrentPlayerCode = result.Code;
                CurrentSecurityToken = result.Token;
                CurrentSecurityTokenPeriod = result.Period;
                PlayerPrefs.Set(kPlayerId, result.Id.ToString());
                PlayerPrefs.Set(kPlayerCode, result.Code);
                PlayerPrefs.Set(kLastDeviceId, SystemInfo.deviceUniqueIdentifier);
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
            if(!PlayerPrefs.HasKey(kPlayerId)) return false;

            var playerId = ulong.Parse(PlayerPrefs.Get(kPlayerId, "0"));
            var deviceId = PlayerPrefs.Get(kLastDeviceId, SystemInfo.deviceUniqueIdentifier);
            var newDeviceId = SystemInfo.deviceUniqueIdentifier != deviceId ? SystemInfo.deviceUniqueIdentifier : null;
            var client = MagicOnionClient.Create<IAccountService>(GlobalChannel);
            Log.Info("LogIn : Call");
            var result = await client.LogIn(new LogInRequest {
                Id = playerId,
                DeviceType = deviceType,
                DeviceId = deviceId,
                NewDeviceId = newDeviceId
            });
            Log.Info("LogIn : End");
            if(result != null) {
                Log.Info($"LogIn : {result}");
                CurrentPlayerId = playerId;
                CurrentSecurityToken = result.Token;
                CurrentSecurityTokenPeriod = result.Period;
                if(newDeviceId != null) {
                    PlayerPrefs.Set(kLastDeviceId, SystemInfo.deviceUniqueIdentifier);
                    PlayerPrefs.Save();
                }
                KeepConnect.Instance.enabled = true;
                return true;
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
            await client.LogOut();
            CurrentPlayerId = 0;
            CurrentSecurityToken = null;
            CurrentSecurityTokenPeriod = DateTime.MaxValue;
            KeepConnect.Instance.enabled = false;
            Log.Info("LogOut");
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
