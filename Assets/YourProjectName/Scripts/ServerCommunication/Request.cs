using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using CustomUnity;
using MessagePack;

namespace YourProjectName
{
    public static class Request
    {
        const string serverUrl = "https://localhost:7142";
        const string apiUrl = serverUrl + "/api/";

        public static ContentType CurrentAccept = ContentType.MessagePack;

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

        public static async UniTask<Models.PlayerAccount> GetPlayerAccount(int index)
        {
            using(var request = UnityWebRequest.Get($"{apiUrl}PlayerAccount/{index}")) {
                request.SetRequestHeader("Accept", CurrentAccept.ToHeaderString());
                await request.SendWebRequest();
                if(request.error == null) {
                    //Log.Info($"Content-Type : {request.GetResponseHeader("Content-Type")}");
                    if(request.GetResponseHeader("Content-Type").Contains("application/x-msgpack")) {
                        return MessagePackSerializer.Deserialize<Models.PlayerAccount>(request.downloadHandler.data);
                    }
                    else {
                        //Log.Info($"source json : {request.downloadHandler.text}");
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<Models.PlayerAccount>(request.downloadHandler.text);
                    }
                }
                else {
                    Log.Error($"GetPlayerAccount : {request.error}");
                }
            }
            return null;
        }
    }
}