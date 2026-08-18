using FluentValidation;

namespace Demo48.LibA;

public sealed class EmailRequest
{
    public string Email { get; set; } = "";
}

public sealed class EmailValidator : AbstractValidator<EmailRequest>
{
    public EmailValidator()
        => RuleFor(x => x.Email).NotEmpty().EmailAddress();
}
