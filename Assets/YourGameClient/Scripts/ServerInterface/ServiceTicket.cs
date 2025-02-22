#nullable disable // Server needs this
using System.Collections.Generic; // Unity needs this
using MessagePack;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    public enum ServiceTicketKind
    {
        LootBox,
        FooBar,
    }

    [MessagePackObject]
    public record ServiceTicket
    {
        [Key(0)]
        public string Id { get; set; } = string.Empty;
        [Key(1)]
        public ServiceTicketKind Kind { get; set; }
        [Key(2)]
        public string DisplayName { get; set; } = string.Empty;
        [Key(3)]
        public string Description { get; set; } = string.Empty;
        [Key(4)]
        public ulong DetailId { get; set; }
        [Key(5)]
        public ulong IconBlobId { get; set; }
    }

    [MessagePackObject]
    public record OwnedServiceTicket
    {
        [Key(0)]
        public string ServiceTicketId { get; set; } = string.Empty;
        [Key(1)]
        public int Count { get; set; }
    }
}
