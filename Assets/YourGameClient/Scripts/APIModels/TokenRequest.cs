using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    [NotMapped]
    [MessagePackObject]
    public record TokenRequest
    {
        [Key(0)]
        public long Id { get; init; }
        [Key(1)]
        public DeviceType DeviceType { get; init; }
        [Key(2)]
        public string DeviceId { get; init; } // Unity's SystemInfo.deviceUniqueIdentifier
        [Key(3)]
        public string NewDeviceId { get; init; } // Unity's SystemInfo.deviceUniqueIdentifier
    }

    [NotMapped]
    [MessagePackObject]
    public record TokenRequestResult
    {
        [Key(0)]
        public long DeviceId { get; init; } // PlayerDevice Table index
        [Key(1)]
        public string Token { get; init; }
    }
}
