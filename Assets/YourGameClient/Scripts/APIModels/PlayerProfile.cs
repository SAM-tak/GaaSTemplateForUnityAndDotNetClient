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
        public ulong Id { get; init; }
        [Key(1)]
        public ulong OwnerId { get; set; }
        [IgnoreMember]
        [ForeignKey("OwnerId")]
        public PlayerAccount Owner { get; init; }
        [Key(2)]
        public string Name { get; set; }
        [Key(3)]
        public string Motto { get; set; }
        [Key(4)]
        public ulong IconBlobId { get; set; }
        [IgnoreMember]
        public IconBlob IconBlob { get; init; }

        public Masked MakeMasked()
        {
            return new Masked {
                Name = Name,
                Motto = Motto,
                IconBlobId = IconBlobId
            };
        }

        [NotMapped]
        [MessagePackObject]
        public record Masked
        {
            [Key(0)]
            public string Name { get; set; }
            [Key(1)]
            public string Motto { get; set; }
            [Key(2)]
            public ulong IconBlobId { get; set; }
        }
    }
}
