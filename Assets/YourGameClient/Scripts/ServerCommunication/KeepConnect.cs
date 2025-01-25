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
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Instantiate()
        {
            if(!Instance) {
                var go = new GameObject(typeof(KeepConnect).FullName);
                DontDestroyOnLoad(go);
                Instance = go.AddComponent<KeepConnect>();
            }
        }

        public static KeepConnect Instance { get; private set; }

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

        void OnDestroy()
        {
            if(Instance == this) Instance = null;
        }

        // 意味がないかも。
        // async void OnApplicationQuit()
        // {
        //     LogInfo("OnApplicationQuit");
        //     if(Request.IsLoggedIn) await Request.LogOut().Timeout(TimeSpan.FromSeconds(3));
        //     if(Instance == this) Instance = null;
        // }
    }
}
