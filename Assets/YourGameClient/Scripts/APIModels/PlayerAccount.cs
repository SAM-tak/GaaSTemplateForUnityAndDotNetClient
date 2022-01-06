using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using UnityEngine;
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
        public List<PlayerDevice> DeviceList { get; init; }
        [Key(4)]
        public PlayerAccountStatus Status { get; set; }
        [Key(5)]
        public DateTime Since { get; init; }
        [Key(6)]
        public DateTime LastLogin { get; set; }
        [Key(7)]
        public DateTime? InactivateDate { get; set; }
        [Key(8)]
        public DateTime? BanDate { get; set; }
        [Key(9)]
        public DateTime? ExpireDate { get; set; }
        [Key(10)]
        public long ProfileId { get; init; }
        [IgnoreMember]
        public PlayerProfile Profile { get; init; }
    }
}
