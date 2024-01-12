namespace CarCareTracker.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsRootUser { get; set; } = false;
        public bool CanAdd { get; set; } = false;
        public bool CanEdit { get; set; } = false;
        public bool CanDelete { get; set; } = false;
        public bool IsActive { get; set; } = false;
    }
}
