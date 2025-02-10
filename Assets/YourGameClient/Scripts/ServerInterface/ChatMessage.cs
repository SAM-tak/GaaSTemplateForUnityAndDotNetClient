#nullable disable
using MessagePack;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    /// <summary>
    /// Message information
    /// </summary>
    [MessagePackObject]
    public struct ChatMessage
    {
        [Key(0)]
        public string UserName { get; set; }
        [Key(1)]
        public string Message { get; set; }
    }
}
