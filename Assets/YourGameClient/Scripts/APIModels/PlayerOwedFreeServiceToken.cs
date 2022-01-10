using System; // Unity needs this
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    [MessagePackObject]
    public record PlayerOwnedFreeServiceToken
    {
        [Key(0)]
        public ulong Id { get; init; }
        [Key(1)]
        public ulong OwnerId { get; init; }
        [IgnoreMember]
        [ForeignKey("OwnerId")]
        public PlayerAccount Owner { get; init; }
        [Key(2)]
        public ConsumableOrigin Origin { get; set; }
        [Key(3)]
        public ConsumableStatus Status { get; set; }
        [Key(4)]
        public DateTime? Period { get; set; }
        [Key(5)]
        public DateTime? UsedDate { get; set; }
        [Key(6)]
        public DateTime? InvalidateDate { get; set; }
        [Key(7)]
        public DateTime? ExpireDate { get; set; }
    }
}
