using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toxiad.IO.Standar.Module
{
    [Flags]
    public enum UserLevel
    {
        System = 0,
        Administrator = 2,
        Operator = 4
    }
    [Table("TOXIAD_BABEL_USERS_BB_USER_V1")]
    public class User
    {
        public const UserLevel AdmSys = UserLevel.Administrator | UserLevel.System;
        public string Avatar { get; set; }
        public string UserName { get; set; }
        [PrimaryKey]
        public string AccountId { get; set; } 
        public string UserDesc { get; set; } 
        public DateTime CreateAt { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime LastHeartBeat { get; set; }
        public string PasswordHash { get; set; }
        public UserLevel AccessLevel { get; set; } 
    }
}
