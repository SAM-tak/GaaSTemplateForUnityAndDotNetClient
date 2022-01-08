using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    [NotMapped]
    [MessagePackObject]
    public record AccountCreationRequest
    {
        [Key(0)]
        public DeviceType DeviceType { get; init; }
        [Key(1)]
        public string DeviceId { get; init; } // Unity's SystemInfo.deviceUniqueIdentifier
        // 何らかのOpenIDを色々受け付ける
    }

    [NotMapped]
    [MessagePackObject]
    public record AccountCreationResult
    {
        [Key(0)]
        public long Id { get; init; }
        [Key(1)]
        public long DeviceId { get; init; } // PlayerDevice Table index
        [Key(2)]
        public string Token { get; init; }
    }
}
