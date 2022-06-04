using System;
using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace YourGameClient
{
    /// <summary>
    /// A component keeps logging into server.<br/>
    /// This renew token automatically.
    /// </summary>
    /// <remarks>
    /// The GameObject has this component will be created automatically. Dont't add this yourself.
    /// </remarks>
    public class KeepConnect : MonoBehaviour
    {
        static Lazy<KeepConnect> LazyInstance => new(() => {
            var go = new GameObject(typeof(KeepConnect).FullName);
            DontDestroyOnLoad(go);
            return go.AddComponent<KeepConnect>();
        });

        public static KeepConnect Instance => LazyInstance.Value;

        void OnEnable()
        {
            StartCoroutine(KeepAlive());
        }

        IEnumerator KeepAlive()
        {
            yield return null;
            while(Request.IsLoggedIn) {
                if((Request.CurrentSecurityTokenPeriod - DateTime.UtcNow).Minutes < 1) {
                    var task = Request.RenewToken();
                    yield return new WaitUntil(() => task.Status.IsCompleted());
                }
                else yield return null;
            }
            enabled = false;
        }

        async void OnDestroy()
        {
            LogInfo("OnDestroy");
            if(Request.IsLoggedIn) await Request.LogOut().Timeout(TimeSpan.FromSeconds(3));
        }

        async void OnApplicationQuit()
        {
            LogInfo("OnApplicationQuit");
            if(Request.IsLoggedIn) await Request.LogOut().Timeout(TimeSpan.FromSeconds(3));
        }
    }
}
