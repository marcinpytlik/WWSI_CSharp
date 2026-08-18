using Minio;
using Minio.DataModel.Args;

namespace Demo40;

public sealed class MinioBlobStore : IBlobStore
{
    private readonly IMinioClient _client;
    private readonly string _bucket;

    public MinioBlobStore(IConfiguration configuration)
    {
        var endpoint = configuration["Minio:Endpoint"] ?? throw new InvalidOperationException("Minio:Endpoint missing.");
        var access = configuration["Minio:AccessKey"] ?? throw new InvalidOperationException("Minio:AccessKey missing.");
        var secret = configuration["Minio:SecretKey"] ?? throw new InvalidOperationException("Minio:SecretKey missing.");
        _bucket = configuration["Minio:Bucket"] ?? "demo40";
        _client = new MinioClient().WithEndpoint(endpoint).WithCredentials(access, secret).WithSSL(false).Build();
    }

    public async Task EnsureBucketAsync(CancellationToken cancellationToken)
    {
        var exists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket), cancellationToken);
        if (!exists)
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket), cancellationToken);
    }

    public async Task<string> PutAsync(string fileName, string contentType, Stream content, CancellationToken cancellationToken)
    {
        await using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);
        buffer.Position = 0;
        var key = $"{Guid.NewGuid():N}-{fileName}";
        await _client.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucket)
            .WithObject(key)
            .WithStreamData(buffer)
            .WithObjectSize(buffer.Length)
            .WithContentType(contentType), cancellationToken);
        return key;
    }

    public async Task<BlobRead?> GetAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            await using var ms = new MemoryStream();
            await _client.GetObjectAsync(new GetObjectArgs()
                .WithBucket(_bucket)
                .WithObject(key)
                .WithCallbackStream(stream => stream.CopyTo(ms)), cancellationToken);
            return new BlobRead(ms.ToArray(), "application/octet-stream", key);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
