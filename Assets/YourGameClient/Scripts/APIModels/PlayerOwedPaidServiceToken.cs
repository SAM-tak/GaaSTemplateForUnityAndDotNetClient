using System; // Unity needs this
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    [MessagePackObject]
    public record PlayerOwnedPaidServiceToken
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

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Id);
            hash.Add(OwnerId);
            hash.Add(Origin);
            hash.Add(Status);
            hash.Add(Period);
            hash.Add(UsedDate);
            hash.Add(InvalidateDate);
            hash.Add(ExpireDate);
            return hash.ToHashCode();
        }
    }
}
