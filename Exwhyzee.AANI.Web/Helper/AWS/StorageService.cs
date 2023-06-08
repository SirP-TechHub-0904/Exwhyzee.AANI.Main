using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Exwhyzee.AANI.Domain.Dtos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Helper.AWS
{
    public class StorageService : IStorageService
    {
        private readonly IConfiguration _config;
        //private readonly IAmazonS3 _client;
        public StorageService(IConfiguration config/*, IAmazonS3 client*/)
        {
            _config = config;
            //_client = client;
        }

        public async Task<S3ResponseDto> UploadFileAsync(Domain.Dtos.S3Object obj, AwsCredentials awsCredentialsValues)
        {
            //var awsCredentialsValues = _config.ReadS3Credentials();
            ////
            // Console.WriteLine($"Key: {awsCredentialsValues.AccessKey}, Secret: {awsCredentialsValues.SecretKey}");

            var credentials = new BasicAWSCredentials(awsCredentialsValues.AccessKey, awsCredentialsValues.SecretKey);

            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast1
            };

            var response = new S3ResponseDto();
            try
            {
                var uploadRequest = new TransferUtilityUploadRequest()
                {
                    InputStream = obj.InputStream,
                    Key = obj.Name,
                    BucketName = obj.BucketName,
                    CannedACL = S3CannedACL.NoACL
                };

                // initialise client
                using var client = new AmazonS3Client(credentials, config);

                // initialise the transfer/upload tools
                var transferUtility = new TransferUtility(client);

                // initiate the file upload
                await transferUtility.UploadAsync(uploadRequest);

                response.StatusCode = 201;
                response.Message = $"{obj.Name} has been uploaded sucessfully";
            }
            catch (AmazonS3Exception s3Ex)
            {
                response.StatusCode = (int)s3Ex.StatusCode;
                response.Message = s3Ex.Message;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<string> DownloadFileAsync(string file, string bucketName, AwsCredentials awsCredentialsValues)
        {


            var credentials = new BasicAWSCredentials(awsCredentialsValues.AccessKey, awsCredentialsValues.SecretKey);

            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast1
            };

            //var response = new S3ResponseDto();
            Stream rs;
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = file,
            };

            var preurl = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = file,
                Expires = DateTime.UtcNow.AddYears(3)
            };
            using var client = new AmazonS3Client(credentials, config);
            var url = client.GetPreSignedURL(preurl);

            return url;
        }

        public async Task<FileReturnResponseDto> UploadFileReturnUrlAsync(Exwhyzee.AANI.Domain.Dtos.S3Object obj, AwsCredentials awsCredentialsValues, string old_key)
        {
            var credentials = new BasicAWSCredentials(awsCredentialsValues.AccessKey, awsCredentialsValues.SecretKey);

            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast1
            };
            var response = new FileReturnResponseDto();
            // initialise client
            using var client = new AmazonS3Client(credentials, config);
            if (!String.IsNullOrEmpty(old_key))
            {
                try
                {
                    //delete object
                    var deleteObjectRequest = new DeleteObjectRequest
                    {
                        BucketName = obj.BucketName,
                        Key = old_key
                    };

                    Console.WriteLine("Deleting an object");
                    await client.DeleteObjectAsync(deleteObjectRequest);
                }
                catch (AmazonS3Exception e)
                {
                    Console.WriteLine("Error encountered on server. Message:'{0}' when deleting an object", e.Message);
                }
                catch (Exception c)
                {

                }
            }
            try
            {
                var uploadRequest = new TransferUtilityUploadRequest()
                {
                    InputStream = obj.InputStream,
                    Key = obj.Name,
                    BucketName = obj.BucketName,
                    CannedACL = S3CannedACL.NoACL
                };


                // initialise the transfer/upload tools
                var transferUtility = new TransferUtility(client);

                // initiate the file upload
                await transferUtility.UploadAsync(uploadRequest);

                //response.StatusCode = 201;
                //response.Message = $"{obj.Name} has been uploaded sucessfully";

                var request = new GetObjectRequest
                {
                    BucketName = obj.BucketName,
                    Key = "obj.Name",
                };

                var preurl = new GetPreSignedUrlRequest
                {
                    BucketName = obj.BucketName,
                    Key = obj.Name,
                    Expires = DateTime.UtcNow.AddYears(3)
                };

                var url = client.GetPreSignedURL(preurl);
                response.Message = "200";
                response.Key = obj.Name;
                response.Url = url;
                return response;

            }
            catch (AmazonS3Exception s3Ex)
            {
                response.Message = s3Ex.StatusCode.ToString() + "<br>" + s3Ex.Message;


            }
            catch (Exception ex)
            {
                response.Message = "error 500 <br>" + ex.Message;
            }
            return response;
        }

        public async Task<FileReturnResponseDto> DeleteObjectAsync(AwsCredentials awsCredentialsValues, string bucket, string key)
        {

            var credentials = new BasicAWSCredentials(awsCredentialsValues.AccessKey, awsCredentialsValues.SecretKey);

            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast1
            };
            var response = new FileReturnResponseDto();
            // initialise client
            using var client = new AmazonS3Client(credentials, config);

            try
            {
                //delete object
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucket,
                    Key = key
                };

                //Console.WriteLine("Deleting an object");
                await client.DeleteObjectAsync(deleteObjectRequest);
                response.Message = "200";
            }
            catch (AmazonS3Exception e)
            {
                //Console.WriteLine("Error encountered on server. Message:'{0}' when deleting an object", e.Message);
            }
            catch (Exception c)
            {

            }
            return response;


        }
    }
}
