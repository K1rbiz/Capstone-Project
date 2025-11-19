using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Capstone_Project_v0._1.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique, NotNull]
        public string Username { get; set; } = string.Empty;
        [NotNull]
        public string PasswordHash { get; set; } = string.Empty;
        [NotNull]
        public string PasswordSalt { get; set; } = string.Empty;

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }
}
