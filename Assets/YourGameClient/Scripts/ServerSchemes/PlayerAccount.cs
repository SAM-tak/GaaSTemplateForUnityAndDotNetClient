#nullable disable
using System; // Unity needs this
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;
using KeyAttribute = MessagePack.KeyAttribute;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    public enum PlayerAccountKind
    {
        [Display(Name = "Guest")]
        Guest,
        [Display(Name = "Special Guest")]
        SpecialGuest,
        [Display(Name = "Community Manager")]
        CommunityManager,
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

#if UNITY_5_3_OR_NEWER
    public record PlayerAccount
    {
        [Display(Name = "ID")]
        public ulong Id { get; init; }
    };
#else
    public record PlayerAccount
    {
        [Display(Name = "ID")]
        public ulong Id { get; init; }
        [Display(Name = "Secret")]
        public ushort Secret { get; set; }
        public List<PlayerDevice> DeviceList { get; init; }
        [Display(Name = "Current DeviceId")]
        public ulong CurrentDeviceId { get; set; }
        [Display(Name = "Kind")]
        public PlayerAccountKind Kind { get; set; }
        [Display(Name = "Status")]
        public PlayerAccountStatus Status { get; set; }
        [Display(Name = "Since")]
        public DateTime? Since { get; set; }
        [Display(Name = "Last Login Time")]
        public DateTime? LastLogin { get; set; }
        [Display(Name = "Inactivate Date")]
        public DateTime? InactivateDate { get; set; }
        [Display(Name = "Ban Date")]
        public DateTime? BanDate { get; set; }
        [Display(Name = "Expire Date")]
        public DateTime? ExpireDate { get; set; }
        [Display(Name = "Profile")]
        public PlayerProfile Profile { get; init; }

        public override int GetHashCode() => (Id, Secret, CurrentDeviceId, Status, Since, LastLogin, InactivateDate, BanDate, ExpireDate).GetHashCode();

        public MaskedPlayerAccount MakeMasked() => new() {
            Id = Id,
            LastLogin = LastLogin,
            Profile = Profile?.MakeMasked()
        };

        public FormalPlayerAccount MakeFormal() => new() {
            Id = Id,
            Code = Code,
            Status = Status,
            Since = Since,
            LastLogin = LastLogin,
            Profile = Profile
        };

        public string Code => IDCoder.Encode(Id, Secret);
    }
#endif

    [NotMapped]
    [MessagePackObject]
    public record MaskedPlayerAccount
    {
        [Key(0)]
        public ulong Id { get; init; }
        [Key(1)]
        public DateTime? LastLogin { get; init; }
        [Key(2)]
        public MaskedPlayerProfile Profile { get; init; }
    }

    [NotMapped]
    [MessagePackObject]
    public record FormalPlayerAccount
    {
        [Key(0)]
        public ulong Id { get; init; }
        [Key(1)]
        public string Code { get; init; }
        [Key(2)]
        public PlayerAccountStatus Status { get; init; }
        [Key(3)]
        public DateTime? Since { get; init; }
        [Key(4)]
        public DateTime? LastLogin { get; init; }
        [Key(5)]
        public PlayerProfile Profile { get; init; }
    }
}
