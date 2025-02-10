#nullable disable
using System; // Unity needs this
using MessagePack;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    [MessagePackObject]
    public record RenewTokenRequestResult
    {
        [Key(0)]
        public string Token { get; init; }
        [Key(1)]
        public DateTime Period { get; init; }
    }
}
