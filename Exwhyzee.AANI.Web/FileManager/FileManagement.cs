using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Exwhyzee.AANI.Domain.Dtos;
using System.Drawing;

namespace Exwhyzee.AANI.Web.FileManager
{
    public class FileManagement : IFileManagement
    {
        private readonly IAmazonS3 _amazonS3;

        public FileManagement(IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
        }
        private const string bucketName = "nipss-bucket";
        //// For simplicity the example creates two objects from the same file.
        //// You specify key names for these objects.
        //private const string keyName1 = "*** key name for first object created ***";
        //private const string keyName2 = "*** key name for second object created ***";
        //private const string filePath = @"*** file path ***";
       // public UploadDto model { get; set; }
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.EUWest3;
        private static IAmazonS3 client;
        public async Task Create(UploadDto model)
        {
            var awsCredentials = new BasicAWSCredentials("AKIA4Y267QYKXZWF3SXJ", "yYtHsaNdHKpm1S10q8M4dCaMUFd86Mo/3mDcOjqh");
            client = new AmazonS3Client(awsCredentials, bucketRegion);
            //client = new AmazonS3Client(bucketRegion);
            UploadAsync(model).Wait();

        }

        public async Task UploadAsync (UploadDto datax) { 
            try
            {
                // 1. Put object-specify only key name for the new object.
                var putRequest1 = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = datax.keyName1,
                    ContentBody = "sample text"
                };

                PutObjectResponse response1 = await _amazonS3.PutObjectAsync(putRequest1);

                // 2. Put the object-set ContentType and add metadata.
                var putRequest2 = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = datax.keyName2,
                    FilePath = datax.filePath,
                    ContentType = "text/plain"
                };

                putRequest2.Metadata.Add("x-amz-meta-title", "someTitle");
                PutObjectResponse response2 = await _amazonS3.PutObjectAsync(putRequest2);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                        "Error encountered ***. Message:'{0}' when writing an object"
                        , e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                    , e.Message);
            }
        }
    }
}
