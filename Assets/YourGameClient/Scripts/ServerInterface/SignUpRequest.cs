#nullable disable
using System; // Unity needs this
using MessagePack;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    [MessagePackObject]
    public record SignUpRequest
    {
        [Key(0)]
        public DeviceType DeviceType { get; init; }
        [Key(1)]
        public string DeviceId { get; init; } // Unity's SystemInfo.deviceUniqueIdentifier
        // 何らかのOpenIDを色々受け付ける
    }

    [MessagePackObject]
    public record SignUpRequestResult
    {
        [Key(0)]
        public string Code { get; init; }
        [Key(1)]
        public string Token { get; init; }
        [Key(2)]
        public DateTime Period { get; init; }
    }
}
