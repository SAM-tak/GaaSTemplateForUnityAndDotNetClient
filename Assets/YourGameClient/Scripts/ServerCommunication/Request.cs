using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using CustomUnity;
using MessagePack;
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

        public static ContentType CurrentAccept = ContentType.MessagePack;
        public static long CurrentPlayerId = 0;
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

        public static async UniTask<PlayerAccount> GetPlayerAccount()
        {
            using(var request = UnityWebRequest.Get($"{apiRootUrl}/{CurrentPlayerId}/PlayerAccounts/self")) {
                request.SetRequestHeader("Accept", CurrentAccept.ToHeaderString());
                request.SetRequestHeader("Authorization", $"Bearer {CurrentSecurityToken}");
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
            }
            return null;
        }

        public static async UniTask<IEnumerable<PlayerAccount>> GetPlayerAccounts(long[] ids)
        {
            using(var request = UnityWebRequest.Get($"{apiRootUrl}/{CurrentPlayerId}/PlayerAccounts?{string.Join('&', ids.Select(ids => "id=" + ids.ToString()))}")) {
                request.SetRequestHeader("Accept", CurrentAccept.ToHeaderString());
                request.SetRequestHeader("Authorization", $"Bearer {CurrentSecurityToken}");
                Log.Info($"CurrentSecurityToken : {CurrentSecurityToken}");

                await request.SendWebRequest();
                if(request.error == null) {
                    Log.Info($"Content-Type : {request.GetResponseHeader("Content-Type")}");
                    if(request.GetResponseHeader("Content-Type").Contains("application/x-msgpack")) {
                        return MessagePackSerializer.Deserialize<IEnumerable<PlayerAccount>>(request.downloadHandler.data);
                    }
                    else if(request.GetResponseHeader("Content-Type").Contains("application/json")) {
                        Log.Info($"source json : {request.downloadHandler.text}");
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<PlayerAccount>>(request.downloadHandler.text);
                    }
                    else {
                        Log.Error($"GetPlayerAccount : Unknown Format");
                        return null;
                    }
                }
                else {
                    Log.Error($"GetPlayerAccount : {request.error}");
                }
            }
            return null;
        }

        public static async UniTask<UnityWebRequest.Result> LogIn()
        {
            if(!PlayerPrefs.HasKey(prefPlayerIdKey)) return UnityWebRequest.Result.ProtocolError;

            var PlayerId = long.Parse(PlayerPrefs.GetString(prefPlayerIdKey));
            var deviceId = PlayerPrefs.GetString(prefLastDeviceIdKey, SystemInfo.deviceUniqueIdentifier);
            var newDeviceId = SystemInfo.deviceUniqueIdentifier != deviceId ? SystemInfo.deviceUniqueIdentifier : null;

            var payload = MessagePackSerializer.Serialize(new TokenRequest {
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
            //var form = new WWWForm();
            //form.AddBinaryData("login", payload, null, "application/x-msgpack");
            using var request = Post($"{apiRootUrl}/token", payload);
            request.SetRequestHeader("Accept", CurrentAccept.ToHeaderString());
            await request.SendWebRequest();
            if(request.error == null) {
                Log.Info($"LogIn : Content-Type : {request.GetResponseHeader("Content-Type")}");
                if(request.GetResponseHeader("Content-Type").Contains("application/x-msgpack")) {
                    CurrentSecurityToken = MessagePackSerializer.Deserialize<string>(request.downloadHandler.data);
                }
                else if(request.GetResponseHeader("Content-Type").Contains("application/json")) {
                    Log.Info($"source json : {request.downloadHandler.text}");
                    CurrentSecurityToken = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(request.downloadHandler.text);
                }
                else if(request.GetResponseHeader("Content-Type").Contains("text/plain")) {
                    CurrentSecurityToken = request.downloadHandler.text;
                }
                CurrentPlayerId = PlayerId;
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

        internal static UnityWebRequest Post(string uri, string json)
        {
            return new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST) {
                uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json)) { contentType = "application/json" },
                downloadHandler = new DownloadHandlerBuffer()
            };
        }

        internal static UnityWebRequest Post(string uri, byte[] msgpack)
        {
            return new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST) {
                uploadHandler = new UploadHandlerRaw(msgpack) { contentType = "application/x-msgpack" },
                downloadHandler = new DownloadHandlerBuffer()
            };
        }

        public static async UniTask<UnityWebRequest.Result> NewAccount()
        {
            var payload = MessagePackSerializer.Serialize(new AccountCreationRequest {
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

            using var request = Post($"{apiRootUrl}/signin", payload);
            request.SetRequestHeader("Accept", CurrentAccept.ToHeaderString());
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
