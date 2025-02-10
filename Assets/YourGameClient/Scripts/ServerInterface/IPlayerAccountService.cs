#nullable disable
using System.Collections.Generic; // Unity needs this
using MagicOnion;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    // Defines .NET interface as a Server/Client IDL.
    // The interface is shared between server and client.
    public interface IPlayerAccountService : IService<IPlayerAccountService>
    {
        UnaryResult<FormalPlayerAccount> GetPlayerAccount();
        UnaryResult<IEnumerable<MaskedPlayerAccount>> GetPlayerAccounts(string[] codes);
        UnaryResult<IEnumerable<MaskedPlayerAccount>> FindPlayerAccounts(int maxCount);
    }
}
