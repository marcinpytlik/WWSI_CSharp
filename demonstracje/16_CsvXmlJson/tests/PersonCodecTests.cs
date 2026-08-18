using Demo16;
using Xunit;

namespace Demo16.Tests;

public class PersonCodecTests
{
    private static readonly Person[] Sample = [new("Ada", 36), new("Alan", 41)];

    [Fact]
    public void Csv_RoundTrip()
        => Assert.Equal(Sample, PersonCodec.FromCsv(PersonCodec.ToCsv(Sample)));

    [Fact]
    public void Json_RoundTrip()
        => Assert.Equal(Sample, PersonCodec.FromJson(PersonCodec.ToJson(Sample)));

    [Fact]
    public void Xml_RoundTrip()
        => Assert.Equal(Sample, PersonCodec.FromXml(PersonCodec.ToXml(Sample)));
}
