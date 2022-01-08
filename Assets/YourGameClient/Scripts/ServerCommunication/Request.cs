using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using MessagePack;
using CustomUnity;
using YourGameServer.Models;

namespace YourGameClient
{
    public static class Request
    {
        public const string serverUrl = "https://localhost:7142";
        public const string apiRootUrl = serverUrl + "/api";
        public const string prefPrefix = "com.yourorg.yourgame.";
        public const string prefPlayerIdKey = prefPrefix + "playerid";
        public const string prefLastDeviceIdKey = prefPrefix + "lastdeviceid";

        public static ContentType CurrentAcceptContentType = ContentType.MessagePack;
        public static ContentType CurrentRequestContentType = ContentType.MessagePack;
        public static long CurrentPlayerId = 0;
        public static long CurrentDeviceId = 0;
        public static string CurrentSecurityToken;

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

        internal static UnityWebRequest NewPostRequest<T>(string uri, T content)
        {
            if(CurrentRequestContentType == ContentType.MessagePack) return WebRequest.Post(uri, MessagePackSerializer.Serialize(content), "application/x-msgpack");
            return WebRequest.PostJson(uri, Newtonsoft.Json.JsonConvert.SerializeObject(content));
        }

        public static async UniTask<PlayerAccount> GetPlayerAccount()
        {
            using var request = UnityWebRequest.Get($"{apiRootUrl}/{CurrentPlayerId}/PlayerAccounts/self");
            request.SetRequestHeader("Accept", CurrentAcceptContentType.ToHeaderString());
            request.SetRequestHeader("Authorization", $"Bearer {CurrentSecurityToken}");
            request.SetRequestHeader("DeviceId", CurrentDeviceId.ToString());
            Log.Info($"CurrentSecurityToken : {CurrentSecurityToken}");

            await request.SendWebRequest();
            if(request.error == null) {
                Log.Info($"Content-Type : {request.GetResponseHeader("Content-Type")}");
                if(request.GetResponseHeader("Content-Type").Contains("application/x-msgpack")) {
                    return MessagePackSerializer.Deserialize<PlayerAccount>(request.downloadHandler.data);
                }
                else if(request.GetResponseHeader("Content-Type").Contains("application/json")) {
                    Log.Info($"source json : {request.downloadHandler.text}");
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerAccount>(request.downloadHandler.text);
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

        public static async UniTask<IEnumerable<PlayerAccount.Masked>> GetPlayerAccounts(long[] ids)
        {
            using var request = UnityWebRequest.Get($"{apiRootUrl}/{CurrentPlayerId}/PlayerAccounts?{string.Join('&', ids.Select(ids => "id=" + ids.ToString()))}");
            request.SetRequestHeader("Accept", CurrentAcceptContentType.ToHeaderString());
            request.SetRequestHeader("Authorization", $"Bearer {CurrentSecurityToken}");
            request.SetRequestHeader("DeviceId", CurrentDeviceId.ToString());
            Log.Info($"CurrentSecurityToken : {CurrentSecurityToken}");

            await request.SendWebRequest();
            if(request.error == null) {
                Log.Info($"Content-Type : {request.GetResponseHeader("Content-Type")}");
                if(request.GetResponseHeader("Content-Type").Contains("application/x-msgpack")) {
                    return MessagePackSerializer.Deserialize<IEnumerable<PlayerAccount.Masked>>(request.downloadHandler.data);
                }
                else if(request.GetResponseHeader("Content-Type").Contains("application/json")) {
                    Log.Info($"source json : {request.downloadHandler.text}");
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<PlayerAccount.Masked>>(request.downloadHandler.text);
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

        public static async UniTask<UnityWebRequest.Result> LogIn()
        {
            if(!PlayerPrefs.HasKey(prefPlayerIdKey)) return UnityWebRequest.Result.ProtocolError;

            var PlayerId = long.Parse(PlayerPrefs.GetString(prefPlayerIdKey));
            var deviceId = PlayerPrefs.GetString(prefLastDeviceIdKey, SystemInfo.deviceUniqueIdentifier);
            var newDeviceId = SystemInfo.deviceUniqueIdentifier != deviceId ? SystemInfo.deviceUniqueIdentifier : null;

            using var request = NewPostRequest($"{apiRootUrl}/token", new TokenRequest {
                Id = PlayerId,
#if UNITY_IOS
                DeviceType = YourGameServer.Models.DeviceType.IOS,
#elif UNITY_ANDROID
                DeviceType = YourGameServer.Models.DeviceType.Android,
#elif UNITY_WEBGL
                DeviceType = YourGameServer.Models.DeviceType.WebGL,
#else
                DeviceType = YourGameServer.Models.DeviceType.StandAlone,
#endif
                DeviceId = deviceId,
                NewDeviceId = newDeviceId
            });
            request.SetRequestHeader("Accept", CurrentAcceptContentType.ToHeaderString());
            await request.SendWebRequest();
            if(request.error == null) {
                Log.Info($"LogIn : Content-Type : {request.GetResponseHeader("Content-Type")}");
                if(request.GetResponseHeader("Content-Type").Contains("application/x-msgpack")) {
                    var ret = MessagePackSerializer.Deserialize<TokenRequestResult>(request.downloadHandler.data);
                    CurrentPlayerId = PlayerId;
                    CurrentDeviceId = ret.DeviceId;
                    CurrentSecurityToken = ret.Token;
                }
                else if(request.GetResponseHeader("Content-Type").Contains("application/json")) {
                    Log.Info($"source json : {request.downloadHandler.text}");
                    var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenRequestResult>(request.downloadHandler.text);
                    CurrentPlayerId = PlayerId;
                    CurrentDeviceId = ret.DeviceId;
                    CurrentSecurityToken = ret.Token;
                }
                else if(request.GetResponseHeader("Content-Type").Contains("text/plain")) {
                    Log.Error($"LogIn : Unknown Format");
                    return request.result;
                }
                if(newDeviceId != null) {
                    PlayerPrefs.SetString(prefLastDeviceIdKey, SystemInfo.deviceUniqueIdentifier);
                    PlayerPrefs.Save();
                }
            }
            else {
                Log.Error($"LogIn : {request.error}");
            }
            return request.result;
        }

        public static async UniTask<UnityWebRequest.Result> NewAccount()
        {
            using var request = NewPostRequest($"{apiRootUrl}/signin", new AccountCreationRequest {
#if UNITY_IOS
                DeviceType = YourGameServer.Models.DeviceType.IOS,
#elif UNITY_ANDROID
                DeviceType = YourGameServer.Models.DeviceType.Android,
#elif UNITY_WEBGL
                DeviceType = YourGameServer.Models.DeviceType.WebGL,
#else
                DeviceType = YourGameServer.Models.DeviceType.StandAlone,
#endif
                DeviceId = SystemInfo.deviceUniqueIdentifier
            });
            request.SetRequestHeader("Accept", CurrentAcceptContentType.ToHeaderString());
            await request.SendWebRequest();
            if(request.error == null) {
                AccountCreationResult ret;
                Log.Info($"SignIn : Content-Type : {request.GetResponseHeader("Content-Type")}");
                if(request.GetResponseHeader("Content-Type").Contains("application/x-msgpack")) {
                    ret = MessagePackSerializer.Deserialize<AccountCreationResult>(request.downloadHandler.data);
                }
                else {
                    Log.Info($"source json : {request.downloadHandler.text}");
                    ret = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountCreationResult>(request.downloadHandler.text);
                }
                CurrentPlayerId = ret.Id;
                CurrentDeviceId = ret.DeviceId;
                CurrentSecurityToken = ret.Token;
                PlayerPrefs.SetString(prefPlayerIdKey, ret.Id.ToString());
                PlayerPrefs.SetString(prefLastDeviceIdKey, SystemInfo.deviceUniqueIdentifier);
                PlayerPrefs.Save();
            }
            else {
                Log.Error($"NewAccount : {request.error}");
            }
            return request.result;
        }
    }
}
