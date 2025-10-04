namespace LabApp;

public static class ReadmeExample 
{
    public static string Echo(string input) => string.IsNullOrWhiteSpace(input) ? "EMPTY" : input.Trim();
}
