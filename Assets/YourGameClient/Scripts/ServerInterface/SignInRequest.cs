#nullable disable
using MessagePack;
using YourGameServer.Models;

namespace YourGameServer.Interface // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    [MessagePackObject]
    public record SignInRequest
    {
        [Key(0)]
        public DeviceType DeviceType { get; init; }
        [Key(1)]
        public string DeviceId { get; init; } // Unity's SystemInfo.deviceUniqueIdentifier
        // 何らかのOpenIDを色々受け付ける
    }

    [MessagePackObject]
    public record SignInRequestResult
    {
        [Key(0)]
        public ulong Id { get; init; }
        [Key(1)]
        public ulong DeviceId { get; init; } // PlayerDevice Table index
        [Key(2)]
        public string Token { get; init; }
    }
}
