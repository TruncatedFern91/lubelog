namespace CarCareTracker.Models
{
    public class AuthCookie
    {
        public UserModel UserData { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
