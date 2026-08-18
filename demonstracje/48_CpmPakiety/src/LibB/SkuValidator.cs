using FluentValidation;

namespace Demo48.LibB;

public sealed class SkuRequest
{
    public string Sku { get; set; } = "";
}

public sealed class SkuValidator : AbstractValidator<SkuRequest>
{
    public SkuValidator()
        => RuleFor(x => x.Sku).NotEmpty().Matches(@"^SKU-\d+$");
}
