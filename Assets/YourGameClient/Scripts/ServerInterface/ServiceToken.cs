#nullable disable // Server needs this
using System.Collections.Generic; // Unity needs this
using MessagePack;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    [MessagePackObject]
    public record OwnedServiceTokens
    {
        [Key(0)]
        public int Free { get; set; }
        [Key(1)]
        public int Paid { get; set; }
        [IgnoreMember]
        public int Count => Free + Paid;
    }
}
