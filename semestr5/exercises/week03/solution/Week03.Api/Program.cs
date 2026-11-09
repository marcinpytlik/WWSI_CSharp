using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<UserCatalog>();
var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

var users = app.MapGroup("/api/v1/users").WithTags("Users");

// Ćwiczenia 1–5: osobne wejściowe DTO; wspólna walidacja DataAnnotations
// w filtrze endpointu; unikalność loginu sprawdzana w usłudze domenowej.
users.MapPost("/", (CreateUserRequest request, UserCatalog catalog) =>
{
    var result = catalog.Create(request);
    if (result is null)
    {
        return Results.Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Login jest już zajęty.",
            detail: "Użytkownik o podanym loginie już istnieje.");
    }

    return Results.Created($"/api/v1/users/{result.Id}", result);
})
.AddEndpointFilter<ValidationFilter<CreateUserRequest>>();

users.MapPut("/{id:int}", (int id, UpdateUserRequest request, UserCatalog catalog) =>
{
    if (id <= 0)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["id"] = ["Id musi być większe od zera."]
        });
    }

    var outcome = catalog.Update(id, request);
    return outcome switch
    {
        UpdateOutcome.NotFound => Results.NotFound(),
        UpdateOutcome.DuplicateLogin => Results.Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Login jest już zajęty."),
        _ => Results.NoContent()
    };
})
.AddEndpointFilter<ValidationFilter<UpdateUserRequest>>();

users.MapGet("/", (UserCatalog catalog) => Results.Ok(catalog.GetAll()));
users.MapGet("/{id:int}", (int id, UserCatalog catalog) =>
    catalog.Get(id) is { } user ? Results.Ok(user) : Results.NotFound());

app.Run();

// Domena: model przechowywany wewnątrz katalogu nie jest DTO wejściowym.
public sealed record User(int Id, string Login, string Email, int Age);
public sealed record UserResponse(int Id, string Login, string Email, int Age);

// Ćwiczenie 1: tylko pola potrzebne do założenia konta; bez Id, CreatedAt, IsAdmin.
public sealed class CreateUserRequest
{
    [Required(ErrorMessage = "Login jest wymagany.")]
    [StringLength(30, MinimumLength = 3,
        ErrorMessage = "Login musi mieć od 3 do 30 znaków.")]
    public string? Login { get; init; }

    [Required(ErrorMessage = "Email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny adres email.")]
    public string? Email { get; init; }

    [Range(18, 120, ErrorMessage = "Wiek musi mieścić się w zakresie 18–120.")]
    public int Age { get; init; }
}

// Drugie DTO pokazuje wielokrotne użycie tego samego mechanizmu walidacji.
public sealed class UpdateUserRequest
{
    [Required(ErrorMessage = "Login jest wymagany.")]
    [StringLength(30, MinimumLength = 3,
        ErrorMessage = "Login musi mieć od 3 do 30 znaków.")]
    public string? Login { get; init; }

    [Required(ErrorMessage = "Email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny adres email.")]
    public string? Email { get; init; }

    [Range(18, 120, ErrorMessage = "Wiek musi mieścić się w zakresie 18–120.")]
    public int Age { get; init; }
}

// Ćwiczenia 2, 4, 5: jeden generyczny filtr dla dowolnego DTO z DataAnnotations.
// ModelState nie jest dostępny automatycznie w Minimal API.
public sealed class ValidationFilter<TRequest> : IEndpointFilter where TRequest : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var model = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if (model is null)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["body"] = ["Wymagane są dane wejściowe."]
            });
        }

        var results = new List<ValidationResult>();
        bool valid = Validator.TryValidateObject(
            model, new ValidationContext(model), results, validateAllProperties: true);

        if (!valid)
        {
            var errors = results
                .SelectMany(error => error.MemberNames.DefaultIfEmpty(string.Empty),
                    (error, member) => new { Member = member, Message = error.ErrorMessage ?? "Niepoprawna wartość." })
                .GroupBy(x => x.Member)
                .ToDictionary(x => x.Key, x => x.Select(e => e.Message).ToArray());

            return Results.ValidationProblem(errors);
        }

        return await next(context);
    }
}

public enum UpdateOutcome { Success, NotFound, DuplicateLogin }

// Ćwiczenie 3: reguła biznesowa niezależna od DataAnnotations.
// Lock pozwala atomowo sprawdzić unikalność i dodać/zmienić login.
public sealed class UserCatalog
{
    private readonly object _gate = new();
    private readonly List<User> _users = [];
    private int _nextId = 1;

    public UserResponse? Create(CreateUserRequest request)
    {
        lock (_gate)
        {
            string login = request.Login!.Trim();
            if (_users.Any(u => string.Equals(u.Login, login, StringComparison.OrdinalIgnoreCase)))
                return null;

            var user = new User(_nextId++, login, request.Email!.Trim(), request.Age);
            _users.Add(user);
            return ToResponse(user);
        }
    }

    public UpdateOutcome Update(int id, UpdateUserRequest request)
    {
        lock (_gate)
        {
            int index = _users.FindIndex(u => u.Id == id);
            if (index < 0) return UpdateOutcome.NotFound;

            string login = request.Login!.Trim();
            if (_users.Any(u => u.Id != id &&
                string.Equals(u.Login, login, StringComparison.OrdinalIgnoreCase)))
                return UpdateOutcome.DuplicateLogin;

            _users[index] = new User(id, login, request.Email!.Trim(), request.Age);
            return UpdateOutcome.Success;
        }
    }

    public UserResponse? Get(int id)
    {
        lock (_gate)
            return _users.Where(u => u.Id == id).Select(ToResponse).FirstOrDefault();
    }

    public UserResponse[] GetAll()
    {
        lock (_gate)
            return _users.Select(ToResponse).ToArray();
    }

    private static UserResponse ToResponse(User user) =>
        new(user.Id, user.Login, user.Email, user.Age);
}
