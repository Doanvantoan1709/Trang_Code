using BaseProject.Model.Entities;
using BaseProject.Service.FileSecurityService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Common.Service;
using Service.Common;
using BaseProject.Model.Entities;

namespace BaseProject.Service.FileSecurityService
{
    public interface IFileSecurityService : IService<FileSecurity>
    {
        Task<PagedList<FileSecurityDto>> GetData(FileSecuritySearch search);
        Task<FileSecurityDto?> GetDto(Guid id);
    }
}
