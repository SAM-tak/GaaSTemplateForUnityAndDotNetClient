using System; // Unity needs this
using System.ComponentModel;
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

        public override string ToString() => $"{{{nameof(Id)}={Id}, {nameof(OwnerId)}={OwnerId}, {nameof(Origin)}={Origin}, {nameof(Status)}={Status}, {nameof(Period)}={Period}, {nameof(UsedDate)}={UsedDate}, {nameof(InvalidateDate)}={InvalidateDate}, {nameof(ExpireDate)}={ExpireDate}}}";

        public void CopyFrom(PlayerOwnedFreeServiceToken token)
        {
            OwnerId = token.OwnerId;
            Origin = token.Origin;
            Status = token.Status;
            Period = token.Period;
            UsedDate = token.UsedDate;
            InvalidateDate = token.InvalidateDate;
            ExpireDate = token.ExpireDate;
        }
    }
}
