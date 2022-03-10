#nullable disable
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    [Table("LootBoxes")]
    [MessagePackObject]
    public record LootBox
    {
        [Key(0)]
        public ulong Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public string ProductName { get; set; }
        [Key(3)]
        public string DisplayName { get; set; }
        [Key(4)]
        public string Description { get; set; }
        [Key(5)]
        public string IconAddress { get; set; }
        [Key(6)]
        public string BannerAddress { get; set; }
    }
}
