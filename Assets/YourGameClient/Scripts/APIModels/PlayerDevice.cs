#nullable disable
using System; // Unity needs this
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#if UNITY_5_3_OR_NEWER
using Newtonsoft.Json;
#else
using System.Text.Json.Serialization;
#endif
using MessagePack;
using KeyAttribute = MessagePack.KeyAttribute;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    public enum DeviceType
    {
        [Display(Name = "iOS")]
        IOS,
        [Display(Name = "Android")]
        Android,
        [Display(Name = "Browser")]
        WebGL,
        [Display(Name = "PC/Mac")]
        StandAlone,
    }

    [MessagePackObject]
    public record PlayerDevice
    {
        [Key(0)]
        [Display(Name = "ID")]
        public ulong Id { get; init; }
        [Key(1)]
        [Display(Name = "Owner Id")]
        public ulong OwnerId { get; set; }
        [IgnoreMember]
        [JsonIgnore]
        [ForeignKey("OwnerId")]
        public PlayerAccount Owner { get; init; }
        [Key(2)]
        [Display(Name = "Device Type")]
        public DeviceType DeviceType { get; set; }
        [Key(3)]
        [Display(Name = "Device ID")]
        public string DeviceId { get; set; }
        [Key(4)]
        [Display(Name = "Since")]
        public DateTime? Since { get; set; }
        [Key(5)]
        [Display(Name = "Last Used Date & Time")]
        public DateTime? LastUsed { get; set; }

        public override int GetHashCode() => HashCode.Combine(Id, OwnerId, DeviceType, DeviceId, Since, LastUsed);

        public override string ToString() => $"{{{nameof(Id)}={Id}, {nameof(OwnerId)}={OwnerId}, {nameof(DeviceType)}={DeviceType}, {nameof(DeviceId)}={DeviceId}, {nameof(Since)}={Since}, {nameof(LastUsed)}={LastUsed}}}";
    }
}
