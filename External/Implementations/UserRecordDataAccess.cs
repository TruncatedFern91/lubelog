using CarCareTracker.External.Interfaces;
using CarCareTracker.Helper;
using CarCareTracker.Models;
using LiteDB;

namespace CarCareTracker.External.Implementations
{
    public class UserRecordDataAccess: IUserRecordDataAccess
    {
        private static string dbName = StaticHelper.DbName;
        private static string tableName = "userrecords";
        public User GetUserRecordByUserName(string userName)
        {
            using (var db = new LiteDatabase(dbName))
            {
                var table = db.GetCollection<User>(tableName);
                var userRecord = table.FindOne(Query.EQ(nameof(User.UserName), userName));
                return userRecord ?? new User();
            };
        }
        public User GetUserRecordById(int userId)
        {
            using (var db = new LiteDatabase(dbName))
            {
                var table = db.GetCollection<User>(tableName);
                var userRecord = table.FindById(userId);
                return userRecord ?? new User();
            };
        }
        public bool SaveUserRecord(User userRecord)
        {
            using (var db = new LiteDatabase(dbName))
            {
                var table = db.GetCollection<User>(tableName);
                table.Upsert(userRecord);
                return true;
            };
        }
        public bool DeleteUserRecord(int userId)
        {
            using (var db = new LiteDatabase(dbName))
            {
                var table = db.GetCollection<User>(tableName);
                table.Delete(userId);
                return true;
            };
        }
    }
}
