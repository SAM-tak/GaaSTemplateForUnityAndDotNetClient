#nullable disable
using System.ComponentModel.DataAnnotations;
using MessagePack;
using KeyAttribute = MessagePack.KeyAttribute;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    public enum ServiceTicketKind
    {
        [Display(Name = "Loot Box")]
        LootBox,
        [Display(Name = "Foo Bar")]
        FooBar,
    }

    [MessagePackObject]
    public record ServiceTicket
    {
        [Key(0)]
        public ulong Id { get; init; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public ServiceTicketKind Kind { get; set; }
        [Key(3)]
        public string ProductName { get; set; }
        [Key(4)]
        public string DisplayName { get; set; }
        [Key(5)]
        public string Description { get; set; }
        [Key(6)]
        public ulong DetailId { get; set; }
        [Key(7)]
        public ulong IconBlobId { get; set; }
    }
}
