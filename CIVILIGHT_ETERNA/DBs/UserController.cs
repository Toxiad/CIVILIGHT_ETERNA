using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Toxiad.IO.Standar;
using Toxiad.IO.Standar.Module;

namespace CIVILIGHT_ETERNA.DBs
{
    public static class Users
    {
        public static async Task<User> LogoutToOpe()
        {
            return await Login("Operator", "");
        }
        public static async Task<User> Login(string account, string password)
        {
            string pwHash = "";
            StringBuilder stringBuilder = new StringBuilder();
            HashAlgorithm.Create("SHA256").ComputeHash(Encoding.UTF8.GetBytes(password)).ToList().ForEach((i) =>
            {
                stringBuilder.Append(i.ToString("x2"));
            });
            pwHash = stringBuilder.ToString();
            //Table<User>()
            var res = await SQLUtil.Instance.MainDB.Table<User>().Where(u => u.AccountId.ToLower() == account.ToLower()).ToListAsync();
            if (res.Count == 0)
            {
                throw new Exception("Login Fail - User Not Exists");
            }
            var success = res.Where(u => u.PasswordHash == pwHash);
            if (success.Count() == 0)
            {
                throw new Exception("Login Fail - Password Verify Error");
            }
            var user = success.First();
            user.LastLogin = DateTime.Now;
            await SQLUtil.Instance.MainDB.UpdateAsync(user);
            return success.First();
        }
    }
}
