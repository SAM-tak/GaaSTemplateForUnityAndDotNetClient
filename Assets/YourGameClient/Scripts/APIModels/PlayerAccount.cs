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
        public ulong Id { get; init; }
        [Key(1), MaxLength(16)]
        public string Code { get; set; }
        [IgnoreMember]
        [JsonIgnore]
        public List<PlayerDevice> DeviceList { get; init; }
        [IgnoreMember]
        [JsonIgnore]
        public ulong CurrentDeviceId { get; set; }
        [Key(2)]
        public PlayerAccountKind Kind { get; set; }
        [Key(3)]
        public PlayerAccountStatus Status { get; set; }
        [Key(4)]
        public DateTime? Since { get; set; }
        [Key(5)]
        public DateTime? LastLogin { get; set; }
        [Key(6)]
        public DateTime? InactivateDate { get; set; }
        [Key(7)]
        public DateTime? BanDate { get; set; }
        [Key(8)]
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

        public void CopyFrom(PlayerAccount account)
        {
            Code = account.Code;
            Status = account.Status;
            Since = account.Since;
            LastLogin = account.LastLogin;
            InactivateDate = account.InactivateDate;
            BanDate = account.BanDate;
            ExpireDate = account.ExpireDate;
        }

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
            public ulong Id { get; init; }
            [Key(1)]
            public PlayerAccountStatus Status { get; set; }
            [Key(2)]
            public DateTime? Since { get; set; }
            [Key(3)]
            public DateTime? LastLogin { get; set; }
            [Key(4)]
            public PlayerProfile.Masked Profile { get; init; }
        }
    }
}
