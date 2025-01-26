#nullable disable

using System.ComponentModel.DataAnnotations;

namespace YourGameServer.Interface // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{

    public enum PlayerAccountKind
    {
        [Display(Name = "Guest")]
        Guest,
        [Display(Name = "Special Guest")]
        SpecialGuest,
        [Display(Name = "Community Manager")]
        CommunityManager,
        [Display(Name = "Staff")]
        Staff,
    }
}