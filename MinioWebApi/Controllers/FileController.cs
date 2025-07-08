using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using miniotest.Services;

namespace miniotest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IMinioService _minioService;
    private readonly string bucketName = "mybucket";

    public FileController(IMinioService minioService)
    {
        _minioService = minioService;
    }

    // اکشن آپلود فایل
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string path)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File not selected.");

        try
        {
            await _minioService.UploadFileAsync(file, path);
            return Ok(new { Message = "File successfully uploaded." });
        }
        catch (MinioException ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Unexpected error: {ex.Message}" });
        }
    }

    [HttpGet("download")]
    public async Task<IActionResult> DownloadFile([FromQuery] string path)
    {
        if (string.IsNullOrEmpty(path))
            return BadRequest("File path is required.");

        try
        {
            var (stream, contentType, fileName) = await _minioService.DownloadFileAsync(path);
            return File(stream, contentType, fileName);
        }
        catch (MinioException ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Unexpected error: {ex.Message}" });
        }
    }

}