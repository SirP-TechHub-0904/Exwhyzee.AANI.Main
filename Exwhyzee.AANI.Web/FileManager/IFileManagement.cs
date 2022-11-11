using Amazon.S3;
using Exwhyzee.AANI.Domain.Dtos;

namespace Exwhyzee.AANI.Web.FileManager
{
    public interface IFileManagement
    {
        Task Create(UploadDto model);

        //Task Edit(ApprovedDevice model);
        //Task<List<ApprovedDevice>> DeviceList();
        //Task Delete(int? id);
        //Task<ApprovedDevice> Get(int? id);
    }
}
