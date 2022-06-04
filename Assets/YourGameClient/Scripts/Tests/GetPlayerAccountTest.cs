using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace YourGameClient
{
    using Extensions;

    public class GetPlayerAccountTest : MonoBehaviour
    {
        public Request.ContentType accept;
        public Request.ContentType contentType;

        public ulong[] ids = new[] { 1UL, 2UL, 3UL, 4UL, 5UL };

        public Button startButton;
        public TMPro.TMP_InputField inputField;

        // Start is called before the first frame update
        async void Start()
        {
            inputField.text = Request.ServerAddr;

            LogInfo($"deviceUniqueIdentifier : {SystemInfo.deviceUniqueIdentifier}");

            while(!FirebaseInitializer.Done) await UniTask.NextFrame();

            LogInfo("Wait for Start Button Push");

            startButton.interactable = true;

            while(!start) await UniTask.NextFrame();

            if(!await Request.LogIn() && !await Request.SignUp()) return;

            Request.CurrentAcceptContentType = accept;
            Request.CurrentRequestContentType = contentType;
            var playerAccount = await Request.GetPlayerAccount();
            LogInfo($"playerAccount : {playerAccount} {playerAccount?.Id} {playerAccount?.Code.ToHyphened()} {playerAccount?.Since}");

            var playerAccounts = await Request.GetPlayerAccounts(ids);
            foreach(var i in playerAccounts) LogInfo($"playerAccount : {i} {i?.Id}");

            LogInfo($"rejason by JsonUtility : {JsonUtility.ToJson(playerAccount)}"); // NG
            LogInfo($"rejason by Newtonsoft : {Newtonsoft.Json.JsonConvert.SerializeObject(playerAccount)}"); // OK
            // JsonUtility非力すぎるのでNewtonsoft.Json使うしかない

            await UniTask.Delay(1000);

            var newToken = await Request.RenewToken();
            LogInfo($"newToken : {newToken}");

            await UniTask.Delay(1000);

            await Request.LogOut();


            //LogInfo("Start");
            //done = true;
        }

        // Update is called once per frame
        void Update()
        {
            // Call the exception-throwing method here so that it's run
            // every frame update
            if(done) ThrowExceptionEvery60Updates2();
        }

        //async void OnDestroy()
        //{
        //    if(Request.IsLoggedIn) await Request.LogOut().Timeout(TimeSpan.FromSeconds(10));
        //}

        bool start;
        bool done;

        public void StartProc()
        {
            LogInfo("StartProc");
            Request.ServerAddr = inputField.text;
            start = true;
        }

        int updatesBeforeException = 60;

        // A method that tests your Crashlytics implementation by throwing an
        // exception every 60 frame updates. You should see non-fatal errors in the
        // Firebase console a few minutes after running your app with this method.
        void ThrowExceptionEvery60Updates()
        {
            if(updatesBeforeException > 0) {
                updatesBeforeException--;
            }
            else {
                // Set the counter to 60 updates
                updatesBeforeException = 60;

                // Throw an exception to test your Crashlytics implementation
                throw new Exception("test exception please ignore");
            }
        }

        void ThrowExceptionEvery60Updates2()
        {
            if(updatesBeforeException > 0) {
                updatesBeforeException--;
            }
            else {
                // Set the counter to 60 updates
                updatesBeforeException = 60;

                // Throw an exception to test your Crashlytics implementation
                throw new ApplicationException("##### test exception please ignore");
            }
        }
    }
}
