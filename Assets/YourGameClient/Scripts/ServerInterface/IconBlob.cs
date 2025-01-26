#nullable disable
using MessagePack;

namespace YourGameServer.Interface // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    [MessagePackObject]
    public record IconBlob
    {
        [Key(0)]
        public string address;
    }
}
