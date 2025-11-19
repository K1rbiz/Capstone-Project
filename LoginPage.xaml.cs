using Capstone_Project_v0._1.Services;

namespace Capstone_Project_v0._1;

public partial class LoginPage : ContentPage
{
    private readonly IAuthService _auth;
    public LoginPage(IAuthService auth)
    {
        InitializeComponent();
        _auth = auth;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var ok = await _auth.LoginAsync(UsernameEntry.Text ?? "", PasswordEntry.Text ?? "");
        if (!ok)
        {
            StatusLabel.Text = "Invalid credentials.";
            return;
        }
        Application.Current!.MainPage = new NavigationPage(new MainPage());
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var created = await _auth.RegisterAsync(UsernameEntry.Text ?? "", PasswordEntry.Text ?? "");
        StatusLabel.Text = created ? "Registered! You can now login." : "Registration failed.";
    }
}