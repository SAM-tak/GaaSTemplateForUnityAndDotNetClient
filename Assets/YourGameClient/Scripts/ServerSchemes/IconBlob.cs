#nullable disable
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    [NotMapped]
    [MessagePackObject]
    public record IconBlob
    {
        [Key(0)]
        public string address;
    }
}
