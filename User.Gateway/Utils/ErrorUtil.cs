using User.Gateway.DTO;

namespace User.Gateway.Utils
{
    public class ErrorUtil
    {
        public static readonly ErrorDto InvalidOperation = new ErrorDto { Status = 10000, Message = "Invalid operation" };
        public static readonly ErrorDto PasswordInvalid = new ErrorDto { Status = 10001, Message = "Password invalid" };
        public static readonly ErrorDto CannotLoginRightNow = new ErrorDto { Status = 10002, Message = "Your account is already login in another device. Logout first or try again later" };
        public static readonly ErrorDto TokenNotFound = new ErrorDto { Status = 10003, Message = "Token not found" };
    }
}
