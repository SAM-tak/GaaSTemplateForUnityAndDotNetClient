#nullable disable
using System.Collections.Generic; // Unity needs this
using MagicOnion;

namespace YourGameServer.Interface // Unity cannot accpect 'namespace YourProjectName.Models;' yet
{
    // Defines .NET interface as a Server/Client IDL.
    // The interface is shared between server and client.
    public interface IPlayerAccountService : IService<IPlayerAccountService>
    {
        UnaryResult<FormalPlayerAccount> GetPlayerAccount();
        UnaryResult<IEnumerable<MaskedPlayerAccount>> GetPlayerAccounts(ulong[] ids);
    }
}
