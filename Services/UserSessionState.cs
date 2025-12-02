namespace Capstone_Project_v0._1.Services
{
    public sealed class UserSessionState
    {
        public int? UserId { get; private set; }
        public string? Username { get; private set; }

        public bool IsAuthenticated => UserId.HasValue;

        public void Login(string username, int userId)
        {
            Username = username;
            UserId = userId;
        }

        public void Logout()
        {
            Username = null;
            UserId = null;
        }
    }
}
