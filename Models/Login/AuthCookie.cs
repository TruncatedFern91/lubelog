namespace CarCareTracker.Models
{
    public class AuthCookie
    {
        public User UserData { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
