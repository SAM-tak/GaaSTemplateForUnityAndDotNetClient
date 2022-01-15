using System; // Unity needs this
using System.Collections.Generic;
using System.ComponentModel;
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
        public ulong Id { get; init; }
        [Key(1)]
        public ulong OwnerId { get; set; }
        [IgnoreMember]
        [JsonIgnore]
        [ForeignKey("OwnerId")]
        public PlayerAccount Owner { get; init; }
        [Key(2)]
        public DeviceType DeviceType { get; set; }
        [Key(3)]
        public string DeviceId { get; set; }
        [Key(4)]
        public DateTime? Since { get; set; }
        [Key(5)]
        public DateTime? LastUsed { get; set; }

        public override int GetHashCode() => HashCode.Combine(Id, OwnerId, DeviceType, DeviceId, Since, LastUsed);

        public override string ToString() => $"{{{nameof(Id)}={Id}, {nameof(OwnerId)}={OwnerId}, {nameof(DeviceType)}={DeviceType}, {nameof(DeviceId)}={DeviceId}, {nameof(Since)}={Since}, {nameof(LastUsed)}={LastUsed}}}";

        public void CopyFrom(PlayerDevice device)
        {
            OwnerId = device.OwnerId;
            DeviceType = device.DeviceType;
            DeviceId = device.DeviceId;
            Since = device.Since;
            LastUsed = device.LastUsed;
        }
    }
}
