
using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Toxiad.IO.Standar.Module;

namespace Toxiad.IO.Standar
{
    public class SQLUtil
    {

        private static SQLUtil instance;
        public static SQLUtil Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SQLUtil();
                }

                return instance;
            }
        }
        private SQLiteAsyncConnection _db;

        private readonly string sqlPath = "./Data/";
        public SQLiteAsyncConnection MainDB => _db;

        public SQLUtil()
        {
            Directory.CreateDirectory(sqlPath);
            try
            {
                var dbs = new SQLiteConnectionString(Path.Combine(sqlPath + "MainDB.db"), true, key: "CIVILIGHT_ETERNA");
                _db = new SQLiteAsyncConnection(dbs);
                _db.EnableWriteAheadLoggingAsync().Wait();
                _db.ExecuteScalarAsync<string>("PRAGMA synchronous=OFF;", Array.Empty<object>()).Wait();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            _db.CreateTableAsync<User>().Wait();
            _db.CreateTableAsync<Config>().Wait();
            if (_db.Table<User>().CountAsync().Result == 0)
            {
                var root = new User()
                {
                    AccountId = "Focalors",
                    UserName = "朶茜雅",
                    UserDesc = "Celestia",
                    Avatar = "Focalors01",
                    CreateAt = DateTime.Now,
                    AccessLevel = UserLevel.System,
                    PasswordHash = "d6e07ade67e960944319e0cf020368791bd22abd8e60c1ef74e6adf6947de99a"
                };
                _db.InsertAsync(root).Wait();
                _db.InsertAsync(new User
                {
                    AccountId = "admin",
                    UserName = "管理员",
                    UserDesc = "一般路过管理员",
                    Avatar = "Shu01",
                    CreateAt = DateTime.Now,
                    AccessLevel = UserLevel.Administrator,
                    PasswordHash = "5517bab653668facef67e08f959d3af9f63e80a0570411d1f4f7c6b0460bc24e"
                }).Wait();
                _db.InsertAsync(new User
                {
                    AccountId = "Operator",
                    UserName = "Operator",
                    UserDesc = "Normal Operator",
                    Avatar = "Babel01",
                    CreateAt = DateTime.Now,
                    AccessLevel = UserLevel.Operator,
                    PasswordHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"
                }).Wait();
            }
            if (_db.Table<Config>().CountAsync().Result == 0)
            {
                _db.InsertAsync(new Config()).Wait();
            }
        }
        
    }
}
