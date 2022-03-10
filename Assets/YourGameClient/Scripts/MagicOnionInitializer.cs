#if UNITY_IOS || UNITY_ANDROID && !UNITY_EDITOR
#else
#define USE_DEV_CERT
#endif
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using MagicOnion.Unity;
using CustomUnity;
using Grpc.Net.Client.Web;
using System.Net.Http;
using Grpc.Core;

namespace YourGameClient
{
    static class MagicOnionInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static async UniTask OnRuntimeInitialize()
        {
#if USE_DEV_CERT
# if UNITY_ANDROID && !UNITY_EDITOR
            Log.Info($"ReadDevCert {Path.Combine(Application.streamingAssetsPath, "ca.crt")}");
            var request = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, "ca.crt"));
            Log.Info("ReadDevCert SendWebRequest");
            await request.SendWebRequest();
            Log.Info($"ReadDevCert SendWebRequest end cert = {request.downloadHandler.text}");
            var cert = request.downloadHandler.text;
            // Initialize gRPC channel provider when the application is loaded.
            GrpcChannelProviderHost.Initialize(new DefaultGrpcChannelProvider(new GrpcCCoreChannelOptions(new[] {
                    // send keepalive ping every 5 second, default is 2 hours
                    new ChannelOption("grpc.keepalive_time_ms", 5000),
                    // keepalive ping time out after 5 seconds, default is 20 seconds
                    new ChannelOption("grpc.keepalive_timeout_ms", 5 * 1000),
                },
                cert != null ? new SslCredentials(request.downloadHandler.text) : null
            )));
# else
            var cert = await File.ReadAllTextAsync(Path.Combine(Application.streamingAssetsPath, "ca.crt"));
            // Initialize gRPC channel provider when the application is loaded.
            GrpcChannelProviderHost.Initialize(new DefaultGrpcChannelProvider(new GrpcCCoreChannelOptions(new[] {
                    // send keepalive ping every 5 second, default is 2 hours
                    new ChannelOption("grpc.keepalive_time_ms", 5000),
                    // keepalive ping time out after 5 seconds, default is 20 seconds
                    new ChannelOption("grpc.keepalive_timeout_ms", 5 * 1000),
                },
                cert != null ? new SslCredentials(cert) : null
            )));
# endif
#else
            // Initialize gRPC channel provider when the application is loaded.
            //GrpcChannelProviderHost.Initialize(new DefaultGrpcChannelProvider(new() {
            //    HttpHandler = new GrpcWebHandler(new HttpClientHandler())
            //}));
            GrpcChannelProviderHost.Initialize(new DefaultGrpcChannelProvider(new[] {
                // send keepalive ping every 5 second, default is 2 hours
                new ChannelOption("grpc.keepalive_time_ms", 5000),
                // keepalive ping time out after 5 seconds, default is 20 seconds
                new ChannelOption("grpc.keepalive_timeout_ms", 5 * 1000),
            }));
            await Task.CompletedTask;
#endif
        }
    }
}
