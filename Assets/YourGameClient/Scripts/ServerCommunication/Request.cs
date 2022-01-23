//#define USE_GRPC_NET_CLINET
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
#if USE_GRPC_NET_CLINET
using System.Net.Http;
using Grpc.Net.Client;
#else
using Grpc.Core;
#endif
using MagicOnion.Client;
using MessagePack;
using CustomUnity;
using YourGameServer.Models;
using YourGameServer.Interface;

namespace YourGameClient
{
    public static class Request
    {
        public const string serverAddr = "localhost";
        public const int serverPort = 7142;
        public const int serverRpcPort = 5019;
        public const int serverTslRpcPort = 7143;
        public static readonly string apiRootUrl = $"https://{serverAddr}:{serverPort}/api";

        public const string prefPrefix = "com.yourorg.yourgame.";
        public const string prefPlayerIdKey = prefPrefix + "playerid";
        public const string prefLastDeviceIdKey = prefPrefix + "lastdeviceid";

        public static ContentType CurrentAcceptContentType = ContentType.MessagePack;
        public static ContentType CurrentRequestContentType = ContentType.MessagePack;
        public static ulong CurrentPlayerId = 0;
        public static string CurrentSecurityToken;
        public static DateTime CurrentSecurityTokenPeriod;

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

        public static async UniTask<PlayerAccount> GetPlayerAccount()
        {
            using var request = UnityWebRequest.Get($"{apiRootUrl}/PlayerAccounts/{CurrentPlayerId}");
            request.SetRequestHeader("Accept", CurrentAcceptContentType.ToHeaderString());
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
            return null;
        }

        public static async UniTask<IEnumerable<MaskedPlayerAccount>> GetPlayerAccounts(long[] ids)
        {
            using var request = UnityWebRequest.Get($"{apiRootUrl}/PlayerAccounts?{string.Join('&', ids.Select(ids => "id=" + ids.ToString()))}");
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
#if USE_GRPC_NET_CLINET
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress($"http://{serverAddr}:{serverRpcPort}", new GrpcChannelOptions {
                HttpHandler = new HttpClientHandler {
                    SslProtocols = System.Security.Authentication.SslProtocols.None
                }
            });
#else
            var channel = new Grpc.Core.Channel(serverAddr, serverRpcPort, ChannelCredentials.Insecure);
#endif
            var client = MagicOnionClient.Create<IAccountService>(channel);
            var result = await client.SignUp(new SignInRequest {
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
            if(result != null) {
                Log.Info($"SignUp : {result}");
                CurrentPlayerId = result.Id;
                CurrentSecurityToken = result.Token;
                CurrentSecurityTokenPeriod = result.Period;
                PlayerPrefs.SetString(prefPlayerIdKey, result.Id.ToString());
                PlayerPrefs.SetString(prefLastDeviceIdKey, SystemInfo.deviceUniqueIdentifier);
                PlayerPrefs.Save();
                return true;
            }
            else {
                Log.Error($"NewAccount : failed");
            }
            return false;
        }

        public static async UniTask<bool> LogIn()
        {
            if(!PlayerPrefs.HasKey(prefPlayerIdKey)) return false;

            var PlayerId = ulong.Parse(PlayerPrefs.GetString(prefPlayerIdKey));
            var deviceId = PlayerPrefs.GetString(prefLastDeviceIdKey, SystemInfo.deviceUniqueIdentifier);
            var newDeviceId = SystemInfo.deviceUniqueIdentifier != deviceId ? SystemInfo.deviceUniqueIdentifier : null;

#if USE_GRPC_NET_CLINET
            var channel = GrpcChannel.ForAddress($"https://{serverAddr}:{serverTslRpcPort}", new GrpcChannelOptions {
                HttpHandler = new HttpClientHandler {
                    // Return `true` to allow certificates that are untrusted/invalid
                    ServerCertificateCustomValidationCallback = (x, y, z, w) => true
                }
            });

            //var channel = GrpcChannel.ForAddress($"http://{serverAddr}:{serverRpcPort}", new GrpcChannelOptions {
            //    HttpHandler = new HttpClientHandler {
            //        SslProtocols = System.Security.Authentication.SslProtocols.None
            //    }
            //});
#else
            //var channel = new Grpc.Core.Channel(serverAddr, serverTslRpcPort, ChannelCredentials.SecureSsl);
            var channel = new Grpc.Core.Channel(serverAddr, serverRpcPort, ChannelCredentials.Insecure);
#endif
            var client = MagicOnionClient.Create<IAccountService>(channel);
            var result = await client.LogIn(new LogInRequest {
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
            if(result != null) {
                Log.Info($"LogIn : {result}");
                CurrentPlayerId = PlayerId;
                CurrentSecurityToken = result.Token;
                CurrentSecurityTokenPeriod = result.Period;
                if(newDeviceId != null) {
                    PlayerPrefs.SetString(prefLastDeviceIdKey, SystemInfo.deviceUniqueIdentifier);
                    PlayerPrefs.Save();
                }
                return true;
            }
            return false;
        }

        public static async UniTask<bool> RenewToken()
        {
#if USE_GRPC_NET_CLINET
            var channel = GrpcChannel.ForAddress($"https://{serverAddr}:{serverTslRpcPort}", new GrpcChannelOptions {
                HttpHandler = new HttpClientHandler {
                    // Return `true` to allow certificates that are untrusted/invalid
                    ServerCertificateCustomValidationCallback = (x, y, z, w) => true
                }
            });
#else
            //var channel = new Grpc.Core.Channel(serverAddr, serverTslRpcPort, ChannelCredentials.SecureSsl);
            var channel = new Grpc.Core.Channel(serverAddr, serverRpcPort, ChannelCredentials.Insecure);
#endif
            var client = MagicOnionClient.Create<IAccountService>(channel, new IClientFilter[] { new AppendHeaderFilter() });
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
#if USE_GRPC_NET_CLINET
            var channel = GrpcChannel.ForAddress($"https://{serverAddr}:{serverTslRpcPort}", new GrpcChannelOptions {
                HttpHandler = new HttpClientHandler {
                    // Return `true` to allow certificates that are untrusted/invalid
                    ServerCertificateCustomValidationCallback = (x, y, z, w) => true
                }
            });
#else
            //var channel = new Grpc.Core.Channel(serverAddr, serverTslRpcPort, ChannelCredentials.SecureSsl);
            var channel = new Grpc.Core.Channel(serverAddr, serverRpcPort, ChannelCredentials.Insecure);
#endif
            var client = MagicOnionClient.Create<IAccountService>(channel, new IClientFilter[] { new AppendHeaderFilter() });
            await client.LogOut();
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
