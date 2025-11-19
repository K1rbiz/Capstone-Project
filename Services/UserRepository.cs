using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Capstone_Project_v0._1.Models;

namespace Capstone_Project_v0._1.Services
{
    public class IUserRepository
    {
        Task InitializeAsync();
        Task<User?> GetUserByUsernameAsync(string username);
        Task<int> AddUserAsync(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly string _dbPath;
        private SQLiteAsyncConnection _conn = default!;

        public UserRepository()
        {
             _dbPath = Path.Combine(FileSystem.AppDataDirectory, "appdata.db");
        }

        public async Task InitializeAsync()
        {
            if (_conn is not null)
                return;
            _conn = new SQLiteAsyncConnection(_dbPath);
            await _conn.CreateTableAsync<User>();
        }

        public Task<User?> GetByUsernameAsync(string username) =>
            _conn.Table<User>().Where(u => u.Username == username).FirstOrDefaultAsync();

        public Task<int> InsertAsync(User user) => _conn.InsertAsync(user);
    }
}
