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
    public enum PlayerAccountKind
    {
        [Display(Name = "Guest")]
        Guest,
        [Display(Name = "Special Guest")]
        SpecialGuest,
        [Display(Name = "Comunity Manager")]
        ComunityManager,
        [Display(Name = "Staff")]
        Staff,
    }

    public enum PlayerAccountStatus
    {
        [Display(Name = "Active")]
        Active,
        [Display(Name = "Inactive")]
        Inactive,
        [Display(Name = "Banned")]
        Banned,
        [Display(Name = "Expired")]
        Expired,
    }

    [MessagePackObject]
    [Index(nameof(Code))]
    public record PlayerAccount
    {
        [Key(0)]
        [Display(Name = "ID")]
        public ulong Id { get; init; }
        [Key(1), MaxLength(16)]
        [Display(Name = "Player Code")]
        public string Code { get; set; }
        [IgnoreMember]
        [JsonIgnore]
        public List<PlayerDevice> DeviceList { get; init; }
        [IgnoreMember]
        [JsonIgnore]
        [Display(Name = "Current DeviceId")]
        public ulong CurrentDeviceId { get; set; }
        [Key(2)]
        [Display(Name = "Kind")]
        public PlayerAccountKind Kind { get; set; }
        [Key(3)]
        [Display(Name = "Status")]
        public PlayerAccountStatus Status { get; set; }
        [Key(4)]
        [Display(Name = "Since")]
        public DateTime? Since { get; set; }
        [Key(5)]
        [Display(Name = "Last Login Time")]
        public DateTime? LastLogin { get; set; }
        [Key(6)]
        [Display(Name = "Inactivate Date")]
        public DateTime? InactivateDate { get; set; }
        [Key(7)]
        [Display(Name = "Ban Date")]
        public DateTime? BanDate { get; set; }
        [Key(8)]
        [Display(Name = "Expire Date")]
        public DateTime? ExpireDate { get; set; }
        [IgnoreMember]
        [JsonIgnore]
        public PlayerProfile Profile { get; init; }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Id);
            hash.Add(Code);
            hash.Add(CurrentDeviceId);
            hash.Add(Status);
            hash.Add(Since);
            hash.Add(LastLogin);
            hash.Add(InactivateDate);
            hash.Add(BanDate);
            hash.Add(ExpireDate);
            return hash.ToHashCode();
        }

        public MaskedPlayerAccount MakeMasked() => new() {
            Id = Id,
            Status = Status,
            Since = Since,
            LastLogin = LastLogin,
            Profile = Profile?.MakeMasked()
        };
    }

    [NotMapped]
    [MessagePackObject]
    public record MaskedPlayerAccount
    {
        [Key(0)]
        public ulong Id { get; init; }
        [Key(1)]
        public PlayerAccountStatus Status { get; init; }
        [Key(2)]
        public DateTime? Since { get; init; }
        [Key(3)]
        public DateTime? LastLogin { get; init; }
        [Key(4)]
        public MaskedPlayerProfile Profile { get; init; }
    }
}
