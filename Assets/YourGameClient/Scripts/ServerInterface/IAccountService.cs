#nullable disable
using MagicOnion;

namespace YourGameServer.Interface
{
    // Defines .NET interface as a Server/Client IDL.
    // The interface is shared between server and client.
    public interface IAccountService : IService<IAccountService>
    {
        // The return type must be `UnaryResult<T>`.
        UnaryResult<LogInRequestResult> LogIn(LogInRequest param);
        UnaryResult<SignInRequestResult> SignUp(SignInRequest signup);
        UnaryResult<RenewTokenRequestResult> RenewToken();
    }
}
