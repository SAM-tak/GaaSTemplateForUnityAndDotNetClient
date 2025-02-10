#nullable disable
using MessagePack;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    /// <summary>
    /// Room participation information
    /// </summary>
    [MessagePackObject]
    public struct ChatJoinRequest
    {
        [Key(0)]
        public string RoomName { get; set; }
        [Key(1)]
        public string UserName { get; set; }
    }
}