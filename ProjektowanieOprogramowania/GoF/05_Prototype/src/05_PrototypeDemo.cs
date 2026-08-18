namespace GoF.Prototype;
public record EmailTemplate(string Subject, string Body)
{
    public EmailTemplate Duplicate() => this with { };
}
