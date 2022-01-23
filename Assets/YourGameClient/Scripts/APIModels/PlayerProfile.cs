#nullable disable
using System; // Unity needs this
using System.ComponentModel.DataAnnotations.Schema;
#if UNITY_5_3_OR_NEWER
using Newtonsoft.Json;
#else
using System.Text.Json.Serialization;
#endif
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
        [JsonIgnore]
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

        public override int GetHashCode() => HashCode.Combine(Id, OwnerId, Name, Motto, IconBlobId);

        public override string ToString() => $"{{{nameof(Id)}={Id}, {nameof(OwnerId)}={OwnerId}, {nameof(Name)}={Name}, {nameof(Motto)}={Motto}, {nameof(IconBlobId)}={IconBlobId}}}";

        public MaskedPlayerProfile MakeMasked() => new() {
            Name = Name,
            Motto = Motto,
            IconBlobId = IconBlobId
        };
    }

    [NotMapped]
    [MessagePackObject]
    public record MaskedPlayerProfile
    {
        [Key(0)]
        public string Name { get; init; }
        [Key(1)]
        public string Motto { get; init; }
        [Key(2)]
        public ulong IconBlobId { get; init; }
    }
}
