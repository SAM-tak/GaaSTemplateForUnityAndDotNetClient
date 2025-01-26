#nullable disable
using System; // Unity needs this
using System.ComponentModel.DataAnnotations;

namespace YourGameServer.Interface // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    public enum DeviceType
    {
        [Display(Name = "iOS")]
        IOS,
        [Display(Name = "Android")]
        Android,
        [Display(Name = "Browser")]
        WebGL,
        [Display(Name = "PC/Mac")]
        StandAlone,
    }
}
