#nullable disable
using MessagePack;
using YourGameServer.Models;

namespace YourGameServer.Interface // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    [MessagePackObject]
    public record RenewTokenRequestResult
    {
        [Key(0)]
        public string Token { get; init; }
    }
}
