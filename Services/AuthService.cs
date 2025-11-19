using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Capstone_Project_v0._1.Models;

   namespace Capstone_Project_v0._1.Services;

   public interface IAuthService
   {
       Task<bool> RegisterUserAsync(string username, string password);
       Task<bool> LoginAsync(string username, string password);
       bool IsAuthenticated { get; }
       User? CurrentUser { get; }
       Task<bool> TryAutoLoginAsync();
       Task LogoutAsync();
   }

  public class AuthService : IAuthService
{
    private readonly IUserRepository _repo;
    private User? _current;
    public bool IsAuthenticated => _current is not null;
    public User? CurrentUser => _current;

    private const int Iterations = 20000;
    private const int SaltSize = 16;
    private const int KeySize = 32;

    public AuthService(IUserRepository repo)
    {
               _repo = repo;
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        await _repo.InitializeAsync();
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        if (await _repo.GetByUsernameAsync(username) is not null)
            return false; // already exists

        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        var hashBytes = HashPassword(password, saltBytes);

        var user = new User
        {
            Username = username.Trim(),
            PasswordSalt = Convert.ToBase64String(saltBytes),
            PasswordHash = Convert.ToBase64String(hashBytes)
        };

        await _repo.InsertAsync(user);
        return true;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        await _repo.InitializeAsync();
        var user = await _repo.GetByUsernameAsync(username.Trim());
        if (user is null) return false;

        var salt = Convert.FromBase64String(user.PasswordSalt);
        var expectedHash = user.PasswordHash;
        var actualHash = Convert.ToBase64String(HashPassword(password, salt));

        if (!CryptographicEquals(expectedHash, actualHash))
            return false;

        _current = user;
        // Persist (for auto login)
        await SecureStorage.SetAsync("lastUser", user.Username);
        return true;
    }

    public async Task<bool> TryAutoLoginAsync()
    {
        var stored = await SecureStorage.GetAsync("lastUser");
        if (string.IsNullOrEmpty(stored)) return false;
        // No password locally; we just rehydrate minimal state (NOT secure for multi-user devices)
        await _repo.InitializeAsync();
        var user = await _repo.GetByUsernameAsync(stored);
        if (user is null) return false;
        _current = user;
        return true;
    }

    public async Task LogoutAsync()
    {
        _current = null;
        SecureStorage.Remove("lastUser");
        await Task.CompletedTask;
    }

    private byte[] HashPassword(string password, byte[] salt)
    {
        using var rfc = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        return rfc.GetBytes(KeySize);
    }

    private bool CryptographicEquals(string a, string b)
    {
        var aBytes = Convert.FromBase64String(a);
        var bBytes = Convert.FromBase64String(b);
        return CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
    }
}

