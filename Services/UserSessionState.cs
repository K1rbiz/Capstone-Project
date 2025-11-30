namespace Capstone_Project_v0._1.Services
{
    public sealed class UserSessionState
    {
        public string? Username { get; private set; }
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Username);

        public void Login(string username)
        {
            Username = username;
        }

        public void Logout()
        {
            Username = null;
        }
    }
}
