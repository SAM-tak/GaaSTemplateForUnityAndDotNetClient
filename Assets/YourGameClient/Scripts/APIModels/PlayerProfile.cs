using System; // Unity needs this
using System.Collections.Generic;
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
        public DateTime? LastUpdate { get; set; }
        [Key(3)]
        public string Name { get; set; }
        [Key(4)]
        public string Motto { get; set; }
        [Key(5)]
        public ulong IconBlobId { get; set; }
        [IgnoreMember]
        public IconBlob IconBlob { get; init; }

        public override int GetHashCode() => HashCode.Combine(Id, OwnerId, Name, Motto, IconBlobId);

        public override string ToString() => $"{{{nameof(Id)}={Id}, {nameof(OwnerId)}={OwnerId}, {nameof(Name)}={Name}, {nameof(Motto)}={Motto}, {nameof(IconBlobId)}={IconBlobId}, {nameof(IconBlob)}={IconBlob}}}";

        public void CopyFrom(PlayerProfile profile)
        {
            Name = profile.Name;
            Motto = profile.Motto;
            IconBlobId = profile.IconBlobId;
        }

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
