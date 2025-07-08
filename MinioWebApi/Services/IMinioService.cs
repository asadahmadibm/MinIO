namespace miniotest.Services
{
    public interface IMinioService
    {
        Task UploadFileAsync(IFormFile file, string path);
        Task<(Stream stream, string contentType, string fileName)> DownloadFileAsync(string path);
        //string GetUrl(string objectName);
        //Task<bool> PathExistsAsync(string path);
        //Task CreatePathAsync(string path);
        //Task<IEnumerable<string>> ListObjectsAsync(string path = "", string fileExtension = "");
    }
}
