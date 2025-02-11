#nullable disable
using System.Threading.Tasks; // Unity needs this
using MagicOnion;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    // A hub must inherit `IStreamingHub<TSelf, TReceiver>`.
    public interface IChatHub : IStreamingHub<IChatHub, IChatHubReceiver>
    {
        Task JoinAsync(ChatJoinRequest request);

        Task LeaveAsync();

        Task SendMessageAsync(string message);
    }

    public interface IChatHubReceiver
    {
        void OnJoin(string userName);
        void OnLeave(string userName);
        void OnSendMessage(ChatMessage message);
    }
}
