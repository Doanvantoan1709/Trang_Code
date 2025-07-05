using BaseProject.Model.Entities;
using BaseProject.Service.FileManagerService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Common.Service;
using Microsoft.AspNetCore.Http;
using BaseProject.Service.FileSecurityService.Dto;
using BaseProject.Model.Entities;
using Service.Common;

namespace BaseProject.Service.FileManagerService
{
    public interface IFileManagerService : IService<FileManager>
    {
        Task CopyFileOrFolder(FileManager sourceItem, FileManager? destinationFolder, List<FileManager> allFile, string rootPath);
        Task<FileManager?> CreateFile(string rootPath, Guid? parentID, IFormFile file);
        Task<FileManager> CreateFileFromDB(Guid? parentID, IFormFile file, string physicalPath);
        Task<bool> CreateFolderFromDB(FileManager folder);
        Task<string> CreatePhysicalFile(string rootPath, IFormFile file);
        Task<bool> DeleteFileorFolders(List<Guid> ids, string rootPath);
        Task<bool> DeleteFileOrFolder(FileManager file, List<FileManager> allFile, string rootPath);
        Task<bool> DeletePhysical(string direction);
        Task<bool> ExistsName(Guid? parentId, string newName);
        Task<List<FileManager>> GetChilds(List<Guid>? ids, List<FileManager> allFiles);
        Task<PagedList<FileManagerDto>> GetData(FileManagerSearch search);
        Task<List<FileManagerDto>> GetDataAll(FileManagerSearch search);
        Task<FileManagerDto?> GetDto(Guid id);
        Task<string> GetPath(Guid? parentID, string name);
        Task MoveFileOrFolder(FileManager sourceItem, FileManager? destinationFolder, List<FileManager> allFile);
        Task UpdatePathChilds(FileManager file);
        Task<FileManager> RenameFileOrFolder(FileManager file, string newName);
        Task<bool> SaveSecurity(List<FileSecurity> fileSecurities, Guid fileID);
        Task<List<FileSecurityDto>> GetShare(Guid fileID, Guid sharedByID);
        Task<string> Download(List<Guid> fileIDs, string rootPath, string rootZipPath);
    }
}
