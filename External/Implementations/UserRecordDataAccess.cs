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
        public List<UserModel> GetUsers()
        {
            using (var db = new LiteDatabase(dbName))
            {
                var table = db.GetCollection<UserModel>(tableName);
                return table.FindAll().ToList();
            };
        }
        public UserModel GetUserRecordByUserName(string userName)
        {
            using (var db = new LiteDatabase(dbName))
            {
                var table = db.GetCollection<UserModel>(tableName);
                var userRecord = table.FindOne(Query.EQ(nameof(UserModel.UserName), userName));
                return userRecord ?? new UserModel();
            };
        }
        public UserModel GetUserRecordById(int userId)
        {
            using (var db = new LiteDatabase(dbName))
            {
                var table = db.GetCollection<UserModel>(tableName);
                var userRecord = table.FindById(userId);
                return userRecord ?? new UserModel();
            };
        }
        public bool SaveUserRecord(UserModel userRecord)
        {
            using (var db = new LiteDatabase(dbName))
            {
                var table = db.GetCollection<UserModel>(tableName);
                table.Upsert(userRecord);
                return true;
            };
        }
        public bool DeleteUserRecord(int userId)
        {
            using (var db = new LiteDatabase(dbName))
            {
                var table = db.GetCollection<UserModel>(tableName);
                table.Delete(userId);
                return true;
            };
        }
    }
}
