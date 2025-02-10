#nullable disable
using MessagePack;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    [MessagePackObject]
    public record IconBlob
    {
        [Key(0)]
        public string address;
    }
}
