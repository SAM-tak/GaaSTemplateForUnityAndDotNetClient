#nullable disable
using System; // Unity needs this
using MessagePack;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    public enum PlayerAccountStatus
    {
        Active,
        Inactive,
        Banned,
        Expired,
    }

    [MessagePackObject]
    public record MaskedPlayerAccount
    {
        [Key(0)]
        public string Code { get; init; }
        [Key(1)]
        public DateTime? LastLogin { get; init; }
        [Key(2)]
        public MaskedPlayerProfile Profile { get; init; }
    }

    [MessagePackObject]
    public record FormalPlayerAccount
    {
        [Key(0)]
        public string Code { get; init; }
        [Key(1)]
        public PlayerAccountStatus Status { get; init; }
        [Key(2)]
        public DateTime? Since { get; init; }
        [Key(3)]
        public DateTime? LastLogin { get; init; }
        [Key(4)]
        public FormalPlayerProfile Profile { get; init; }
    }

    [MessagePackObject]
    public record FormalPlayerProfile
    {
        [Key(0)]
        public string OwnerCode { get; set; }
        [Key(1)]
        public DateTime? LastUpdate { get; set; }
        [Key(2)]
        public string Name { get; set; }
        [Key(3)]
        public string Motto { get; set; }
        [Key(4)]
        public ulong IconBlobId { get; set; }
    }

    [MessagePackObject]
    public record MaskedPlayerProfile
    {
        [Key(0)]
        public string Name { get; init; }
        [Key(1)]
        public string Motto { get; init; }
        [Key(2)]
        public ulong IconBlobId { get; init; }
    }
}
