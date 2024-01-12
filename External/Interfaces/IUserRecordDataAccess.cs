using CarCareTracker.Models;

namespace CarCareTracker.External.Interfaces
{
    public interface IUserRecordDataAccess
    {
        public User GetUserRecordByUserName(string userName);
        public User GetUserRecordById(int userId);
        public bool SaveUserRecord(User userRecord);
        public bool DeleteUserRecord(int userId);
    }
}
