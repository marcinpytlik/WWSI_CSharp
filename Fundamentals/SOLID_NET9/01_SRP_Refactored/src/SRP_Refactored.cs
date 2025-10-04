namespace Solid.SRP.Ref;

public record User(string Email);
public interface IUserValidator { void Validate(string email); }
public interface IUserRepository { void Add(User user); bool Exists(string email); }
public interface IWelcomeMailer { void Send(string email); }

public class UserValidator : IUserValidator
{
    public void Validate(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            throw new ArgumentException("Invalid email");
    }
}

public class InMemoryUserRepository : IUserRepository
{
    private readonly HashSet<string> _db = new();
    public void Add(User user) => _db.Add(user.Email);
    public bool Exists(string email) => _db.Contains(email);
}

public class ConsoleWelcomeMailer : IWelcomeMailer
{
    public void Send(string email) { /* nop for demo */ }
}

public class UserService
{
    private readonly IUserRepository _repo;
    private readonly IUserValidator _validator;
    private readonly IWelcomeMailer _mailer;

    public UserService(IUserRepository repo, IUserValidator validator, IWelcomeMailer mailer)
        => (_repo, _validator, _mailer) = (repo, validator, mailer);

    public void Register(string email)
    {
        _validator.Validate(email);
        _repo.Add(new User(email));
        _mailer.Send(email);
    }
}
