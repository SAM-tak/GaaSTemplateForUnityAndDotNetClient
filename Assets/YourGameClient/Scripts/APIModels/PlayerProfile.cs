using System; // Unity needs this
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    [MessagePackObject]
    public record PlayerProfile
    {
        [Key(0)]
        public long Id { get; init; }
        [Key(1)]
        public long OwnerId { get; init; }
        [IgnoreMember]
        [ForeignKey("OwnerId")]
        public PlayerAccount Owner { get; init; }
        [Key(2)]
        public string Name { get; set; }
        [Key(3)]
        public string Motto { get; set; }
        [Key(4)]
        public ulong IconBlobId { get; init; }
        [IgnoreMember]
        public IconBlob IconBlob { get; init; }
    }
}
