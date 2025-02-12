#nullable disable // Server needs this
using MessagePack;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    [MessagePackObject]
    public record IconBlob
    {
        [Key(0)]
        public string Url { get; init; }
    }
}
