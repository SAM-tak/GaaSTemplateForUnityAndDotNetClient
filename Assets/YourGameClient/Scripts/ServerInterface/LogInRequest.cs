#nullable disable
using MessagePack;
using YourGameServer.Models;

namespace YourGameServer.Interface // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    [MessagePackObject]
    public record LogInRequest
    {
        [Key(0)]
        public ulong Id { get; set; }
        [Key(1)]
        public DeviceType DeviceType { get; set; }
        [Key(2)]
        public string DeviceId { get; set; } // Unity's SystemInfo.deviceUniqueIdentifier
        [Key(3)]
        public string NewDeviceId { get; set; } // Unity's SystemInfo.deviceUniqueIdentifier
    }

    [MessagePackObject]
    public record LogInRequestResult
    {
        [Key(0)]
        public ulong DeviceId { get; init; } // PlayerDevice Table index
        [Key(1)]
        public string Token { get; init; }
    }
}
