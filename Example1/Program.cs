// See https://aka.ms/new-console-template for more information
using Minio;

Console.WriteLine("Hello, World!");
var endpoint = "192.168.25.59:9000";
var accessKey = "HZWguglvXRqp9tbw6eig";
var secretKey = "btgDgJqjEox3fRSg1KNrzKwSlMcB5Dh2nYvnxlhs";
var secure = false;
// Initialize the client with access credentials.
IMinioClient minio = new MinioClient()
                                    .WithEndpoint(endpoint)
                                    .WithCredentials(accessKey, secretKey)
                                    .Build();

// Create an async task for listing buckets.
var getListBucketsTask = await minio.ListBucketsAsync().ConfigureAwait(false);

// Iterate over the list of buckets.
foreach (var bucket in getListBucketsTask.Buckets)
{
    Console.WriteLine(bucket.Name + " " + bucket.CreationDateDateTime);
}
Console.ReadLine();