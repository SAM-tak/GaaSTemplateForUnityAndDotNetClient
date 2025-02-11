using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using MagicOnion;
using MagicOnion.Client;
using YourGameServer.Game.Interface;
using YourGameClient.Extensions;

namespace YourGameClient
{
    public class ChatClient : MonoBehaviour, IChatHubReceiver
    {
        public Button joinOrLeaveButton;
        public Button sendMessageButton;
        public TMP_Text joinOrLeaveButtonText;
        public TMP_InputField input;
        public TMP_InputField nameInput;
        public TMP_InputField addrInputField;

        readonly ChatHub _chatHub = new();
        bool _start = false;

        const string RoomName = "Sample Room";

        async void Start()
        {
            addrInputField.text = Request.ServerAddr;

            LogInfo($"deviceUniqueIdentifier : {SystemInfo.deviceUniqueIdentifier}");

            await UniTask.WaitUntil(() => MagicOnionInitializer.Done);
            await UniTask.WaitUntil(() => FirebaseInitializer.Done);

            LogInfo("Wait for Start Button Push");

            joinOrLeaveButton.interactable = true;

            while(!_start) await UniTask.NextFrame();

            addrInputField.interactable = false;
            nameInput.interactable = false;
            LogInfo($"server : {Request.ServerAddr}");
            if(!await Request.LogIn() && !await Request.SignUp()) return;
            LogInfo($"current player : {Request.CurrentPlayerCode.ToHyphened()}");
            await _chatHub.StartAsync(RoomName, nameInput.text);
            UpdateUI(true);
        }

        async void OnDestroy()
        {
            // Clean up Hub and channel
            await _chatHub.ShutdownAsync();
            if(Request.IsLoggedIn) await Request.LogOut().Timeout(TimeSpan.FromSeconds(1));
            await Request.ShutdownAsync();
        }

        void UpdateUI(bool connected)
        {
            joinOrLeaveButton.interactable = true;
            sendMessageButton.interactable = connected;
            input.interactable = connected;
            joinOrLeaveButtonText.text = connected ? "Leave" : "Join";
        }

        #region Client -> Server (Streaming)
        public async void JoinOrLeave()
        {
            if(!_start) {
                _start = true;
                return;
            }
            if(_chatHub.HasJoined) {
                await _chatHub.LeaveAsync();
                UpdateUI(false);
            }
            else {
                await _chatHub.JoinAsync();
                UpdateUI(true);
            }
        }

        public async void SendMessage()
        {
            if(!_chatHub.HasJoined) {
                LogError("Not joined yet");
                return;
            }

            input.interactable = false;
            sendMessageButton.interactable = false;

            await _chatHub.SendMessageAsync(input.text);

            input.text = string.Empty;
            input.interactable = true;
            sendMessageButton.interactable = true;
        }
        #endregion

        #region Server -> Client (Streaming)
        public void OnJoin(string name)
        {
            LogInfo($"\n<color=grey>{name} entered the room.</color>");
        }

        public void OnLeave(string name)
        {
            LogInfo($"\n<color=grey>{name} left the room.</color>");
        }

        public void OnSendMessage(ChatMessage message)
        {
            LogInfo($"\n{message.UserName}:{message.Message}");
        }
        #endregion
    }
}
