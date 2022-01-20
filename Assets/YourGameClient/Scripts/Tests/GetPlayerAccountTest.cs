using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YourGameClient
{
    public class GetPlayerAccountTest : MonoBehaviour
    {
        public Request.ContentType accept;
        public Request.ContentType contentType;

        public long[] ids = new[] { 1L, 2L, 3L, 4L, 5L };

        // Start is called before the first frame update
        async void Start()
        {
            LogInfo($"deviceUniqueIdentifier : {SystemInfo.deviceUniqueIdentifier}");

            if(!await Request.LogIn() && !await Request.SignUp()) return;

            Request.CurrentAcceptContentType = accept;
            Request.CurrentRequestContentType = contentType;
            var playerAccount = await Request.GetPlayerAccount();
            LogInfo($"playerAccount : {playerAccount} {playerAccount?.Id} {playerAccount?.Since}");
            
            var playerAccounts = await Request.GetPlayerAccounts(ids);
            foreach(var i in playerAccounts) LogInfo($"playerAccount : {i} {i?.Id} {i?.Since}");

            LogInfo($"rejason by JsonUtility : {JsonUtility.ToJson(playerAccount)}"); // NG
            LogInfo($"rejason by Newtonsoft : {Newtonsoft.Json.JsonConvert.SerializeObject(playerAccount)}"); // OK
            // JsonUtility非力すぎるのでNewtonsoft.Json使うしかない

            var newToken = await Request.RenewToken();
            LogInfo($"newToken : {newToken}");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
