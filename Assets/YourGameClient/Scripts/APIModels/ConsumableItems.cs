using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    public enum ConsumableStatus
    {
        [Description("Available")]
        Available,
        [Description("Consumed")]
        Consumed,
        [Description("Invalid")]
        Invalid,
        [Description("Expired")]
        Expired,
    }

    public enum ConsumableOrigin
    {
        [Description("Log-In Reward")]
        LogInReward,
        [Description("Mission Reward")]
        MissionReward,
        [Description("Quest Reward")]
        QuestReward,
        [Description("Achievement Reward")]
        AchievementReward,
        [Description("Insentive")]
        Insentive,
        [Description("Wide-Distribution")]
        Distribution,
        [Description("Loot Box")]
        LootBox,
        [Description("Compensation")]
        Compensation,
        [Description("apologize")]
        apologize,
        [Description("App Store")]
        AppStore,
        [Description("Google Play")]
        GooglePlay,
        [Description("DMM")]
        DMM,
        [Description("Steam")]
        Steam,
    }

    [MessagePackObject]
    public record ServiceToken
    {
        [Key(0)]
        public ulong Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public string ProductName { get; set; }
        [Key(3)]
        public string DisplayName { get; set; }
        [Key(4)]
        public string Description { get; set; }
        [Key(5)]
        public ulong IconBlobId { get; init; }
    }

    [MessagePackObject]
    public record ServiceTicket
    {
        [Key(0)]
        public ulong Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public string ProductName { get; set; }
        [Key(3)]
        public string DisplayName { get; set; }
        [Key(4)]
        public string Description { get; set; }
        [Key(5)]
        public ulong IconBlobId { get; init; }
    }
}
