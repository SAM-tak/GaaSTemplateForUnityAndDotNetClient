using System; // Unityで必要
using MessagePack;

namespace YourProjectName.Models // Unityでnamespace YourProjectName.Models; とはできないので
{
    public enum PlayerStatus
    {
        Active,
        Inactive,
        Banned,
        Expired,
    }

    [MessagePackObject]
    public record PlayerAccount
    {
        [Key(0)]
        public long ID { get; set; }
        [Key(1)]
        public PlayerStatus Status { get; set; }
        [Key(2)]
        public DateTime Since { get; set; }
        [Key(3)]
        public DateTime LastLogin { get; set; }
        [Key(4)]
        public DateTime? InactivateDate { get; set; }
        [Key(5)]
        public DateTime? BanDate { get; set; }
        [Key(6)]
        public DateTime? ExpireDate { get; set; }
    }

}
