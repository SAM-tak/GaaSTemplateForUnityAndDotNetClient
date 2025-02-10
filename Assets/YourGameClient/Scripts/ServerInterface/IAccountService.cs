#nullable disable
using MagicOnion;
using MessagePack;

namespace YourGameServer.Game.Interface // Unity cannot use file-scope namespace yet
{
    // Defines .NET interface as a Server/Client IDL.
    // The interface is shared between server and client.
    public interface IAccountService : IService<IAccountService>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signup"></param>
        /// <returns></returns>
        UnaryResult<SignUpRequestResult> SignUp(SignUpRequest signup);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        UnaryResult<LogInRequestResult> LogIn(LogInRequest param);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        UnaryResult<RenewTokenRequestResult> RenewToken();

        /// <summary>
        /// Log out
        /// </summary>
        /// <returns>none</returns>
        UnaryResult<Nil> LogOut();
    }
}
