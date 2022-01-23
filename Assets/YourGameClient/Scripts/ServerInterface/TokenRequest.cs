#nullable disable
using System; // Unity needs this
using MessagePack;

namespace YourGameServer.Interface // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    [MessagePackObject]
    public record RenewTokenRequestResult
    {
        [Key(0)]
        public string Token { get; init; }
        [Key(1)]
        public DateTime Period { get; init; }
    }
}
