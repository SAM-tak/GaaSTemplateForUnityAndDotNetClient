#nullable disable // Server needs this

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    public enum DeviceType
    {
        IOS,
        Android,
        WebGL,
        StandAlone,
    }
}