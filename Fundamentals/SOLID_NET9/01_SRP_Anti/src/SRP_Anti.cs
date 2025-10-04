namespace Solid.SRP.Anti;

public class UserService
{
    // łamie SRP: waliduje + zapisuje + wysyła e-mail + loguje
    public List<string> Log { get; } = new();
    private readonly List<string> _db = new();

    public void Register(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            throw new ArgumentException("Invalid email");

        _db.Add(email);
        SendWelcome(email);
        Log.Add($"Registered:{email}");
    }

    private void SendWelcome(string email)
    {
        // udawany e-mail
        Log.Add($"WelcomeMail:{email}");
    }

    public bool Exists(string email) => _db.Contains(email);
}
