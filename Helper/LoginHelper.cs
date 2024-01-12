using CarCareTracker.External.Interfaces;
using CarCareTracker.Models;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace CarCareTracker.Helper
{
    public interface ILoginHelper
    {
        UserModel ValidateUserCredentials(LoginModel credentials);
    }
    public class LoginHelper: ILoginHelper
    {
        private IUserRecordDataAccess _userRecordDataAccess;
        public LoginHelper(IUserRecordDataAccess userRecordDataAccess)
        {
            _userRecordDataAccess = userRecordDataAccess;
        }
        public UserModel ValidateUserCredentials(LoginModel credentials)
        {
            var configFileContents = System.IO.File.ReadAllText(StaticHelper.UserConfigPath);
            var existingUserConfig = System.Text.Json.JsonSerializer.Deserialize<UserConfig>(configFileContents);
            if (existingUserConfig is not null)
            {
                //create hashes of the login credentials.
                var hashedUserName = Sha256_hash(credentials.UserName);
                var hashedPassword = Sha256_hash(credentials.Password);
                //compare against stored hash.
                if (hashedUserName == existingUserConfig.UserNameHash &&
                    hashedPassword == existingUserConfig.UserPasswordHash)
                {
                    return new UserModel()
                    {
                        Id = -1, //negative one for root user.
                        UserName = credentials.UserName,
                        IsRootUser = true,
                        CanAdd = true,
                        CanEdit = true,
                        CanDelete = true
                    };
                } else //authenticate via DB, not a root user.
                {
                    var userToVerifyAgainst = _userRecordDataAccess.GetUserRecordByUserName(credentials.UserName);
                    if (hashedPassword == userToVerifyAgainst.Password && userToVerifyAgainst.IsActive)
                    {
                        return userToVerifyAgainst;
                    }
                }
            }
            return new UserModel();
        }
        private static string Sha256_hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
