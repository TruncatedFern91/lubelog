using CarCareTracker.External.Interfaces;
using CarCareTracker.Helper;
using CarCareTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CarCareTracker.Controllers
{
    public class LoginController : Controller
    {
        private IDataProtector _dataProtector;
        private ILoginHelper _loginHelper;
        private IUserRecordDataAccess _userRecordDataAccess;
        private readonly ILogger<LoginController> _logger;
        public LoginController(
            ILogger<LoginController> logger,
            IDataProtectionProvider securityProvider,
            ILoginHelper loginHelper,
            IUserRecordDataAccess userRecordDataAccess
            ) 
        {
            _dataProtector = securityProvider.CreateProtector("login");
            _logger = logger;
            _userRecordDataAccess = userRecordDataAccess;
            _loginHelper = loginHelper;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginModel credentials)
        {
            if (string.IsNullOrWhiteSpace(credentials.UserName) ||
                string.IsNullOrWhiteSpace(credentials.Password))
            {
                return Json(false);
            }
            //compare it against hashed credentials
            try
            {
                var loginIsValid = _loginHelper.ValidateUserCredentials(credentials);
                if (loginIsValid.Id != default)
                {
                    AuthCookie authCookie = new AuthCookie
                    {
                        UserData = loginIsValid,
                        ExpiresOn = DateTime.Now.AddDays(credentials.IsPersistent ? 30 : 1)
                    };
                    var serializedCookie = JsonSerializer.Serialize(authCookie);
                    var encryptedCookie = _dataProtector.Protect(serializedCookie);
                    Response.Cookies.Append("ACCESS_TOKEN", encryptedCookie, new CookieOptions { Expires = new DateTimeOffset(authCookie.ExpiresOn) });
                    return Json(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on saving config file.");
            }
            return Json(false);
        }
        [Authorize] //User must already be logged in to do this.
        [HttpPost]
        public IActionResult CreateLoginCreds(LoginModel credentials)
        {
            try
            {
                var configFileContents = System.IO.File.ReadAllText(StaticHelper.UserConfigPath);
                var existingUserConfig = JsonSerializer.Deserialize<UserConfig>(configFileContents);
                if (existingUserConfig is not null)
                {
                    //create hashes of the login credentials.
                    var hashedUserName = Sha256_hash(credentials.UserName);
                    var hashedPassword = Sha256_hash(credentials.Password);
                    //copy over settings that are off limits on the settings page.
                    existingUserConfig.EnableAuth = true;
                    existingUserConfig.UserNameHash = hashedUserName;
                    existingUserConfig.UserPasswordHash = hashedPassword;
                }
                System.IO.File.WriteAllText(StaticHelper.UserConfigPath, JsonSerializer.Serialize(existingUserConfig));
                return Json(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on saving config file.");
            }
            return Json(false);
        }
        [Authorize]
        [HttpPost]
        public IActionResult DestroyLoginCreds()
        {
            try
            {
                var configFileContents = System.IO.File.ReadAllText(StaticHelper.UserConfigPath);
                var existingUserConfig = JsonSerializer.Deserialize<UserConfig>(configFileContents);
                if (existingUserConfig is not null)
                {
                    //copy over settings that are off limits on the settings page.
                    existingUserConfig.EnableAuth = false;
                    existingUserConfig.UserNameHash = string.Empty;
                    existingUserConfig.UserPasswordHash = string.Empty;
                }
                System.IO.File.WriteAllText(StaticHelper.UserConfigPath, JsonSerializer.Serialize(existingUserConfig));
                //destroy any login cookies.
                Response.Cookies.Delete("ACCESS_TOKEN");
                return Json(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on saving config file.");
            }
            return Json(false);
        }
        [Authorize]
        [HttpPost]
        public IActionResult LogOut()
        {
            Response.Cookies.Delete("ACCESS_TOKEN");
            return Json(true);
        }
        [Authorize(Roles = nameof(UserModel.IsRootUser))]
        [HttpGet]
        public IActionResult GetUsers()
        {
            var result = _userRecordDataAccess.GetUsers();
            return PartialView("_Users", result);
        }
        [Authorize(Roles = nameof(UserModel.IsRootUser))]
        [HttpPost]
        public IActionResult CreateUser(LoginModel credentials)
        {
            if (string.IsNullOrWhiteSpace(credentials.UserName) || string.IsNullOrWhiteSpace(credentials.Password))
            {
                return Json(false);
            }
            var hashedPassword = Sha256_hash(credentials.Password);
            var result = _userRecordDataAccess.SaveUserRecord(new UserModel() { UserName = credentials.UserName, Password = hashedPassword, IsActive = true });
            return Json(result);
        }
        [Authorize(Roles = nameof(UserModel.IsRootUser))]
        [HttpPost]
        public IActionResult UpdateUser(UserModel userModel)
        {
            //retrieve password from db.
            var existingUser = _userRecordDataAccess.GetUserRecordById(userModel.Id);
            userModel.UserName = existingUser.UserName;
            userModel.Password = existingUser.Password;
            var result = _userRecordDataAccess.SaveUserRecord(userModel);
            return Json(result);
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
