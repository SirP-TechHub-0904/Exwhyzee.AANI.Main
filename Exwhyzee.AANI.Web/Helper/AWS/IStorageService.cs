using Exwhyzee.AANI.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Helper.AWS
{
   public interface IStorageService
    {
        Task<S3ResponseDto> UploadFileAsync(S3Object obj, AwsCredentials awsCredentialsValues);
        Task<FileReturnResponseDto> UploadFileReturnUrlAsync(S3Object obj, AwsCredentials awsCredentialsValues, string old_key);
        Task<FileReturnResponseDto> DeleteObjectAsync(AwsCredentials awsCredentialsValues, string bucket, string key);
        Task<string> DownloadFileAsync(string file, string bucketName, AwsCredentials awsCredentialsValues);
    }
}
