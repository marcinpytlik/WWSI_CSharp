using Xunit;

namespace Task06_ReadCsv.Tests;

public sealed class CsvReaderTests
{
    [Fact]
    public void ReadCsv_ReadsRows()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".csv");
        File.WriteAllLines(path, new[] { "id,name", "1,Ala", "2,Ola" });

        var rows = Task06_ReadCsv.CsvReader.ReadCsv(path, skipHeader: true).ToList();

        Assert.Equal(2, rows.Count);
        Assert.Equal(new[] { "1", "Ala" }, rows[0]);

        File.Delete(path);
    }
}
