using BaseProject.Model.Entities;
using BaseProject.Model.Entities;
using BaseProject.Service.Common;
using BaseProject.Service.Constant;

namespace BaseProject.Service.FileManagerService.Dto
{
    public class FileManagerDto : FileManager
    {
        public FilePermissionDto? Permission { get; set; }
    }
}
