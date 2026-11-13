using System.Buffers.Binary;
using System.Globalization;
using System.Text;

namespace QueueCacheDemo.Errors;

// RabbitMQ headers may be materialized as numbers or bytes depending on the client.
// Unknown/malformed values fail closed (no unlimited retry loops).
public static class RetryAttemptParser
{
    public static int Parse(object? value)
    {
        if (value is null) return 0;
        int parsed;
        switch (value)
        {
            case int number: parsed = number; break;
            case long longNumber when longNumber is >= int.MinValue and <= int.MaxValue:
                parsed = (int)longNumber; break;
            case short shortNumber: parsed = shortNumber; break;
            case byte byteNumber: parsed = byteNumber; break;
            case string text when int.TryParse(text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var textNumber):
                parsed = textNumber; break;
            case byte[] encodedBytes when int.TryParse(Encoding.UTF8.GetString(encodedBytes),
                NumberStyles.Integer, CultureInfo.InvariantCulture, out var bytesNumber):
                parsed = bytesNumber; break;
            case byte[] binaryBytes when binaryBytes.Length == 4:
                parsed = BinaryPrimitives.ReadInt32BigEndian(binaryBytes); break;
            default: return 3;
        }
        return parsed is >= 0 and <= 3 ? parsed : 3;
    }
}
