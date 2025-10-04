namespace GoF.FactoryMethod;

public interface IExporter { string Write(); }
public class PdfExporter : IExporter { public string Write() => "PDF"; }
public class HtmlExporter : IExporter { public string Write() => "HTML"; }

public abstract class Report
{
    public abstract IExporter CreateExporter();
    public string Export() => CreateExporter().Write();
}

public class PdfReport : Report
{
    public override IExporter CreateExporter() => new PdfExporter();
}

public class HtmlReport : Report
{
    public override IExporter CreateExporter() => new HtmlExporter();
}
