using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using miniotest.DTO;
using miniotest.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// تنظیمات MinIO
builder.Services.Configure<MinIOSettings>(
    builder.Configuration.GetSection("MinIO"));

// ثبت MinIO Client
builder.Services.AddSingleton<IMinioClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<MinIOSettings>>().Value;

    return new MinioClient()
        .WithEndpoint(settings.Endpoint)
        .WithCredentials(settings.AccessKey, settings.SecretKey)
        .WithSSL(settings.Secure)
        .Build();
});

// ثبت MinIO Service
builder.Services.AddSingleton<IMinioService, MinioService>();

var app = builder.Build();

// فعال‌سازی Swagger در حالت توسعه
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
// اطمینان از وجود Bucket
await EnsureBucketExists(app.Services);
app.Run();
// متد برای اطمینان از وجود Bucket
static async Task EnsureBucketExists(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var minioClient = scope.ServiceProvider.GetRequiredService<IMinioClient>();
    var settings = scope.ServiceProvider.GetRequiredService<IOptions<MinIOSettings>>().Value;

    try
    {
        var bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(settings.BucketName);

        var found = await minioClient.BucketExistsAsync(bucketExistsArgs);

        if (!found)
        {
            var makeBucketArgs = new MakeBucketArgs()
                .WithBucket(settings.BucketName);

            await minioClient.MakeBucketAsync(makeBucketArgs);
            Console.WriteLine($"Bucket '{settings.BucketName}' created successfully.");
        }
        else
        {
            Console.WriteLine($"Bucket '{settings.BucketName}' already exists.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error ensuring bucket exists: {ex.Message}");
    }
}