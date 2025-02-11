using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using YourGameServer.Game.Interface;
using YourGameClient.Extensions;

namespace YourGameClient
{
    public class ChatTest : MonoBehaviour
    {
        public Button joinOrLeaveButton;
        public Button sendMessageButton;
        public TMP_Text joinOrLeaveButtonText;
        public TMP_InputField input;
        public TMP_InputField addrInputField;

        readonly ChatClient _chatClient = new();
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

            await UniTask.WaitUntil(() => _start);

            addrInputField.interactable = false;
            LogInfo($"server : {Request.ServerAddr}");
            if(!await Request.LogIn() && !await Request.SignUp()) return;
            LogInfo($"current player : {Request.CurrentPlayerCode.ToHyphened()}");
            await _chatClient.StartAsync(RoomName);
            UpdateUI(true);
        }

        async void OnDestroy()
        {
            // Clean up Hub and channel
            await _chatClient.ShutdownAsync();
            if(Request.IsLoggedIn) await Request.LogOut().Timeout(TimeSpan.FromSeconds(1));
            await Request.ShutdownAsync();
        }

        void UpdateUI(bool connected)
        {
            joinOrLeaveButton.interactable = true;
            sendMessageButton.interactable = connected;
            input.interactable = connected;
            joinOrLeaveButtonText.text = connected ? "Leave" : "Join";
            _chatClient.onRecievedMessage -= OnRecievedMessage;
            if(connected) _chatClient.onRecievedMessage += OnRecievedMessage;
        }

        public async void JoinOrLeave()
        {
            if(!_start) {
                Request.ServerAddr = addrInputField.text;
                _start = true;
                return;
            }
            if(_chatClient.HasJoined) {
                await _chatClient.LeaveAsync();
                UpdateUI(false);
            }
            else {
                await _chatClient.JoinAsync();
                UpdateUI(true);
            }
        }

        public async void SendMessage()
        {
            if(!_chatClient.HasJoined) {
                LogError("Not joined yet");
                return;
            }

            input.interactable = false;
            sendMessageButton.interactable = false;

            await _chatClient.SendMessageAsync(input.text);

            input.text = string.Empty;
            input.interactable = true;
            sendMessageButton.interactable = true;
        }

        public void OnRecievedMessage(ChatMessage message)
        {
            ChatLogDataSource.AddLog(new() {
                dateTime = message.DateTime,
                self = Request.CurrentPlayerCode == message.Member.PlayerCode,
                name = message.Member.PlayerName,
                message = message.Message
            });
        }
    }
}
