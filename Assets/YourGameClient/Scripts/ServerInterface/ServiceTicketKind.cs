#nullable disable
using System.ComponentModel.DataAnnotations;

namespace YourGameServer.Interface // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    public enum ServiceTicketKind
    {
        [Display(Name = "Loot Box")]
        LootBox,
        [Display(Name = "Foo Bar")]
        FooBar,
    }
}
