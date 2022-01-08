using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;
using Microsoft.EntityFrameworkCore;
#if UNITY_5_3_OR_NEWER
using Newtonsoft.Json;
#else
using System.Text.Json.Serialization;
#endif
using KeyAttribute = MessagePack.KeyAttribute;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    public enum PlayerAccountStatus
    {
        [Description("Active")]
        Active,
        [Description("Inactive")]
        Inactive,
        [Description("Banned")]
        Banned,
        [Description("Expired")]
        Expired,
    }

    [MessagePackObject]
    [Index(nameof(Luid))]
    public record PlayerAccount
    {
        [Key(0)]
        public long Id { get; init; }
        [Key(1), MaxLength(16)]
        public string Luid { get; init; }
        [IgnoreMember]
        [JsonIgnore]
        public List<PlayerDevice> DeviceList { get; init; }
        [IgnoreMember]
        [JsonIgnore]
        public long CurrentDeviceId { get; set; }
        [Key(2)]
        public PlayerAccountStatus Status { get; set; }
        [Key(3)]
        public DateTime Since { get; init; }
        [Key(4)]
        public DateTime LastLogin { get; set; }
        [Key(5)]
        public DateTime? InactivateDate { get; set; }
        [Key(6)]
        public DateTime? BanDate { get; set; }
        [Key(7)]
        public DateTime? ExpireDate { get; set; }
        [IgnoreMember]
        [JsonIgnore]
        public PlayerProfile Profile { get; init; }

        public Masked MakeMasked()
        {
            return new Masked {
                Id = Id,
                Status = Status,
                Since = Since,
                LastLogin = LastLogin,
                Profile = Profile?.MakeMasked()
            };
        }

        [NotMapped]
        [MessagePackObject]
        public record Masked
        {
            [Key(0)]
            public long Id { get; init; }
            [Key(1)]
            public PlayerAccountStatus Status { get; set; }
            [Key(2)]
            public DateTime Since { get; init; }
            [Key(3)]
            public DateTime LastLogin { get; set; }
            [Key(4)]
            public PlayerProfile.Masked Profile { get; init; }
        }
    }
}
