using System; // Unity needs this
using MessagePack;

namespace YourProjectName.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
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
        public long ID { get; set; } // cannot use 'init', because MessagePack's generated static deserializer dosen't support it on Unity
        // but all member must be as a property, ASP.NET Json serializer ignore member field even though public
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
