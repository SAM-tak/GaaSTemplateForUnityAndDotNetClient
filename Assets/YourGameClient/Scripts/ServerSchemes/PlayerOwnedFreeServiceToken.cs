#nullable disable
using System; // Unity needs this
using System.ComponentModel.DataAnnotations.Schema;
#if UNITY_5_3_OR_NEWER
using Newtonsoft.Json;
#else
using System.Text.Json.Serialization;
#endif
using MessagePack;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    [MessagePackObject]
    public record PlayerOwnedFreeServiceToken
    {
        [Key(0)]
        public ulong Id { get; init; }
        [Key(1)]
        public ulong OwnerId { get; set; }
        [IgnoreMember]
        [JsonIgnore]
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

        public override int GetHashCode() => HashCode.Combine(Id, OwnerId, Origin, Status, Period, UsedDate, InvalidateDate, ExpireDate);

        public override string ToString() => $"{{{nameof(Id)}={Id}, {nameof(OwnerId)}={OwnerId}, {nameof(Origin)}={Origin}, {nameof(Status)}={Status}, {nameof(Period)}={Period}, {nameof(UsedDate)}={UsedDate}, {nameof(InvalidateDate)}={InvalidateDate}, {nameof(ExpireDate)}={ExpireDate}}}";
    }
}
