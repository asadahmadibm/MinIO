namespace miniotest.Services;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

public class MinioService : IMinioService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public MinioService(IMinioClient minioClient, IConfiguration configuration)
    {
        _minioClient = minioClient;
        _bucketName = configuration["Minio:BucketName"];
        EnsureBucketExists().Wait();
    }

    private async Task EnsureBucketExists()
    {
        bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
        if (!found)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
        }
    }

    public async Task UploadFileAsync(IFormFile file, string path)
    {
        var objectName = string.IsNullOrEmpty(path) ? file.FileName : $"{path.TrimEnd('/')}/{file.FileName}";

        using var stream = file.OpenReadStream();

        var putArgs = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(file.ContentType);

        await _minioClient.PutObjectAsync(putArgs);
    }

    public async Task<(Stream stream, string contentType, string fileName)> DownloadFileAsync(string path)
    {
        var memoryStream = new MemoryStream();

        var stat = await _minioClient.StatObjectAsync(new StatObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(path));

        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(path)
            .WithCallbackStream(stream =>
            {
                stream.CopyTo(memoryStream);
            }));

        memoryStream.Seek(0, SeekOrigin.Begin);

        return (memoryStream, stat.ContentType ?? "application/octet-stream", Path.GetFileName(path));
    }
}

