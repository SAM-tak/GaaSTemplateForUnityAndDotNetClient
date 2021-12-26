using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YourProjectName
{
    public class GetPlayerAccountTest : MonoBehaviour
    {

        public Request.ContentType contentType;

        [Range(1, 10)]
        public int index = 1;

        // Start is called before the first frame update
        async void Start()
        {
            Request.CurrentAccept = contentType;
            var playerAccount = await Request.GetPlayerAccount(index);
            LogInfo($"playerAccount : {playerAccount} {playerAccount?.ID} {playerAccount?.Since}");

            LogInfo($"rejason by JsonUtility : {JsonUtility.ToJson(playerAccount)}"); // NG
            LogInfo($"rejason by Newtonsoft : {Newtonsoft.Json.JsonConvert.SerializeObject(playerAccount)}"); // OK
            // JsonUtility非力すぎるのでNewtonsoft.Json使うしかない
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
