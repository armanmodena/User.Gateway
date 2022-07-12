using User.Gateway.DTO.Error;

namespace User.Gateway.Utils
{
    public class ErrorUtil
    {
        public static readonly Error InvalidOperation = new Error { Status = 10000, Message = "Invalid operation" };
        public static readonly Error PasswordInvalid = new Error { Status = 10001, Message = "Password invalid" };
        public static readonly Error CannotLoginRightNow = new Error { Status = 10002, Message = "Your account is already login in another device. Logout first or try again later" };
        public static readonly Error TokenNotFound = new Error { Status = 10003, Message = "Token not found" };
    }
}
