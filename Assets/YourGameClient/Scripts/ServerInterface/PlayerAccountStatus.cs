#nullable disable

using System.ComponentModel.DataAnnotations;

namespace YourGameServer.Interface // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    public enum PlayerAccountStatus
    {
        [Display(Name = "Active")]
        Active,
        [Display(Name = "Inactive")]
        Inactive,
        [Display(Name = "Banned")]
        Banned,
        [Display(Name = "Expired")]
        Expired,
    }
}