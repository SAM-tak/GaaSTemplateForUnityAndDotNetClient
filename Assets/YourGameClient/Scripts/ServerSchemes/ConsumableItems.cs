#nullable disable
using System.ComponentModel.DataAnnotations;

namespace YourGameServer.Models // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    public enum ConsumableStatus
    {
        [Display(Name = "Available")]
        Available,
        [Display(Name = "Consumed")]
        Consumed,
        [Display(Name = "Invalid")]
        Invalid,
        [Display(Name = "Expired")]
        Expired,
    }

    public enum ConsumableOrigin
    {
        [Display(Name = "Not Specified")]
        NotSpecified,
        [Display(Name = "Log-In Reward")]
        LogInReward,
        [Display(Name = "Mission Reward")]
        MissionReward,
        [Display(Name = "Quest Reward")]
        QuestReward,
        [Display(Name = "Achievement Reward")]
        AchievementReward,
        [Display(Name = "Insentive")]
        Insentive,
        [Display(Name = "Wide-Distribution")]
        Distribution,
        [Display(Name = "Loot Box")]
        LootBox,
        [Display(Name = "Compensation")]
        Compensation,
        [Display(Name = "Apologies")]
        Apologies,
        [Display(Name = "App Store")]
        AppStore,
        [Display(Name = "Google Play")]
        GooglePlay,
        [Display(Name = "DMM")]
        DMM,
        [Display(Name = "Steam")]
        Steam,
    }
}
