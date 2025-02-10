#nullable disable
using MagicOnion;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    // A hub must inherit `IStreamingHub<TSelf, TReceiver>`.
    public interface IChatHub : IStreamingHub<IChatHub, IChatHubReceiver>
    {
        Task JoinAsync(ChatJoinRequest request);

        Task LeaveAsync();

        Task SendMessageAsync(string message);

        Task GenerateException(string message);
    }

    public interface IChatHubReceiver
    {
        void OnJoin(string userName);
        void OnLeave(string userName);
        void OnSendMessage(ChatMessage message);
    }
}
