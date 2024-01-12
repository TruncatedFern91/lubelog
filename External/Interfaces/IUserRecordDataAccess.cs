using CarCareTracker.Models;

namespace CarCareTracker.External.Interfaces
{
    public interface IUserRecordDataAccess
    {
        public List<UserModel> GetUsers();
        public UserModel GetUserRecordByUserName(string userName);
        public UserModel GetUserRecordById(int userId);
        public bool SaveUserRecord(UserModel userRecord);
        public bool DeleteUserRecord(int userId);
    }
}
