using System; // Unity needs this
using System.Collections.Generic;
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
    public enum DeviceType
    {
        [Description("iOS")]
        IOS,
        [Description("Android")]
        Android,
        [Description("Browser")]
        WebGL,
        [Description("PC/Mac")]
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

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, OwnerId, DeviceType, DeviceId, Since, LastUsed);
        }
    }
}
