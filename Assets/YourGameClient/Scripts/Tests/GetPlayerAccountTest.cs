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

        public string[] codes;

        public Button startButton;
        public TMPro.TMP_InputField inputField;

        // Start is called before the first frame update
        async void Start()
        {
            inputField.text = Request.ServerAddr;

            LogInfo($"deviceUniqueIdentifier : {SystemInfo.deviceUniqueIdentifier}");

            await UniTask.WaitUntil(() => MagicOnionInitializer.Done);
            await UniTask.WaitUntil(() => FirebaseInitializer.Done);

            LogInfo("Wait for Start Button Push");

            startButton.interactable = true;

            await UniTask.WaitUntil(() => _start);

            LogInfo("Start");

            if(!await Request.LogIn() && !await Request.SignUp()) return;
            LogInfo($"current player : {Request.CurrentPlayerCode.ToHyphened()}");

            Request.CurrentAcceptContentType = accept;
            Request.CurrentRequestContentType = contentType;

            try {
                var playerAccount = await Request.GetPlayerAccount();
                LogInfo($"playerAccount : {playerAccount}");
                LogInfo($"rejason by JsonUtility : {JsonUtility.ToJson(playerAccount)}"); // NG
                LogInfo($"rejason by Newtonsoft : {Newtonsoft.Json.JsonConvert.SerializeObject(playerAccount)}"); // OK
                // JsonUtility非力すぎるのでNewtonsoft.Json使うしかない
            }
            catch(Exception e) {
                LogException(e);
            }

            try {
                var playerAccounts = await Request.FindPlayerAccounts(5);
                foreach(var i in playerAccounts) LogInfo($"playerAccount : {i} {i?.Code}");
            }
            catch(Exception e) {
                LogException(e);
            }

            try {
                var playerAccounts = await Request.GetPlayerAccounts(codes);
                foreach(var i in playerAccounts) LogInfo($"playerAccount : {i} {i?.Code}");
            }
            catch(Exception e) {
                LogException(e);
            }

            await UniTask.Delay(1000);

            try {
                var newToken = await Request.RenewToken();
                LogInfo($"newToken : {newToken}");
            }
            catch(Exception e) {
                LogException(e);
            }

            await UniTask.Delay(1000);

            try {
                await Request.LogOut();
            }
            catch(Exception e) {
                LogException(e);
            }
            //done = true;
        }

        // Update is called once per frame
        void Update()
        {
            // Call the exception-throwing method here so that it's run
            // every frame update
            if(_done) ThrowExceptionEvery60Updates2();
        }

        async void OnDestroy()
        {
            if(Request.IsLoggedIn) await Request.LogOut().Timeout(TimeSpan.FromSeconds(1));
            await Request.ShutdownAsync();
        }

        bool _start;
        bool _done;

        public void StartProc()
        {
            LogInfo("StartProc");
            Request.ServerAddr = inputField.text;
            _start = true;
            startButton.interactable = false;
            inputField.interactable = false;
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
