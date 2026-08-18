using Demo40;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var testing = builder.Configuration.GetValue("Testing", false);

if (testing)
{
    builder.Services.AddSingleton<IBlobStore, InMemoryBlobStore>();
}
else
{
    var cs = builder.Configuration.GetConnectionString("App")
             ?? throw new InvalidOperationException("Missing ConnectionStrings:App");
    builder.Services.AddDbContext<FilesDb>(o => o.UseSqlServer(cs));
    builder.Services.AddSingleton<IBlobStore, MinioBlobStore>();
}

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FilesDb>();
    await SqlRetry.WaitAsync(() => db.Database.EnsureCreatedAsync(), logger);
    if (scope.ServiceProvider.GetRequiredService<IBlobStore>() is MinioBlobStore minio)
        await minio.EnsureBucketAsync(CancellationToken.None);
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/api/v1/files", async (FilesDb db) =>
    await db.Files.AsNoTracking().OrderByDescending(f => f.Id).Select(f => new
    {
        f.Id,
        f.FileName,
        f.ContentType,
        f.Size
    }).ToListAsync());

app.MapPost("/api/v1/files", async (IFormFile file, FilesDb db, IBlobStore blobs) =>
{
    if (file.Length <= 0)
        return Results.BadRequest(new { error = "Empty file." });
    await using var stream = file.OpenReadStream();
    var key = await blobs.PutAsync(file.FileName, file.ContentType ?? "application/octet-stream", stream, CancellationToken.None);
    var stored = new StoredFile
    {
        FileName = file.FileName,
        ContentType = file.ContentType ?? "application/octet-stream",
        BlobKey = key,
        Size = file.Length
    };
    db.Files.Add(stored);
    await db.SaveChangesAsync();
    return Results.Created($"/api/v1/files/{stored.Id}", new { stored.Id, stored.FileName, stored.Size });
}).DisableAntiforgery();

app.MapGet("/api/v1/files/{id:int}", async (int id, FilesDb db, IBlobStore blobs) =>
{
    var meta = await db.Files.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
    if (meta is null) return Results.NotFound();
    var blob = await blobs.GetAsync(meta.BlobKey, CancellationToken.None);
    if (blob is null) return Results.NotFound();
    return Results.File(blob.Bytes, meta.ContentType, meta.FileName);
});

app.Run();

public partial class Program;
