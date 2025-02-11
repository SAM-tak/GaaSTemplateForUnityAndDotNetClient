#nullable disable // Server needs this
using System; // Unity needs this
using System.Threading.Tasks; // Unity needs this
using MagicOnion;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    // A hub must inherit `IStreamingHub<TSelf, TReceiver>`.
    public interface IChatHub : IStreamingHub<IChatHub, IChatHubReceiver>
    {
        Task<Guid> JoinAsync(ChatJoinRequest request);

        Task LeaveAsync();

        Task SendMessageAsync(string message);
    }

    public interface IChatHubReceiver
    {
        void OnJoin(ChatMember member);
        void OnLeave(ChatMember member);
        void OnRecievedMessage(ChatMessage message);
    }
}
