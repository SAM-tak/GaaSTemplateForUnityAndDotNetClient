using System; // Unity needs this
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    [MessagePackObject]
    public record ServiceToken
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
        public ulong IconBlobId { get; set; }

        public void CopyFrom(ServiceToken token)
        {
            Name = token.Name;
            ProductName = token.ProductName;
            DisplayName = token.DisplayName;
            Description = token.Description;
            IconBlobId = token.IconBlobId;
        }
    }
}