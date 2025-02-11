#nullable disable // Server needs this
using System; // Unity needs this
using MessagePack;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    /// <summary>
    /// Message information
    /// </summary>
    [MessagePackObject]
    public record ChatMessage
    {
        [Key(0)]
        public ChatMember Member { get; set; }
        [Key(1)]
        public DateTime DateTime { get; set; }
        [Key(2)]
        public string Message { get; set; }
    }
}
