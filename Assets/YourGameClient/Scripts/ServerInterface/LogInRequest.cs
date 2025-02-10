#nullable disable
using System; // Unity needs this
using MessagePack;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    [MessagePackObject]
    public record LogInRequest
    {
        [Key(0)]
        public string Code { get; init; }
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
        public string Token { get; init; }
        [Key(1)]
        public DateTime Period { get; init; }
    }
}
