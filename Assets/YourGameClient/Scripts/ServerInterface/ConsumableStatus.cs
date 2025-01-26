#nullable disable
using System.ComponentModel.DataAnnotations;

namespace YourGameServer.Interface // Unity cannot accpect 'namespace YourProjectName.Models;' yet
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
}
