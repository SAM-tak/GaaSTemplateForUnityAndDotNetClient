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
                _apiRootUrl = null;
                if(_globalChannel != null) {
                    _globalChannel.Dispose();
                    _globalChannel = null;
                }
                serverAddr = value;
            }
        }
        static string _apiRootUrl = null;
#if USE_API_TSL
        public const int serverPort = 7142;
        public const string scheme = "https";
#else
        public const int serverPort = 5018;
        public const string scheme = "http";
#endif
        public static string ApiRootUrl {
            get {
                _apiRootUrl ??= $"{scheme}://{ServerAddr}:{serverPort}/api";
                return _apiRootUrl;
            }
        }
#if USE_RPC_TSL
        public const int serverRpcPort = 7143;
        public const string rpcScheme = "https";
#else
        public const int serverRpcPort = 5019;
        public const string rpcScheme = "http";
#endif
        static GrpcChannelx _globalChannel = null;
        public static GrpcChannelx GlobalChannel {
            get {
                _globalChannel ??= GrpcChannelx.ForAddress($"{rpcScheme}://{ServerAddr}:{serverRpcPort}");
                return _globalChannel;
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
            request.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();
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
            request.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();
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
                bool needs_save = false;
                if(LatestPlayerCode != result.Code) {
                    PlayerPrefs.Set(PlayerCode, result.Code);
                    needs_save = true;
                }
                if(newDeviceId != null) {
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

    public class AcceptAllCertificatesSignedWithASpecificPublicKey : CertificateHandler
    {
        // This will validate the certificate using the built-in logic
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // If you want to ignore the certificate validation and accept all certificates, return true
            // WARNING: This is insecure and should be used only for testing
            return true;
        }
    }
}
