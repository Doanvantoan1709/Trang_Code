using BaseProject.Model.Entities;
using BaseProject.Service.Constant;


namespace BaseProject.Service.FileSecurityService.Dto
{
    public class FileSecurityDto : FileSecurity
    {
        public string SharedToName { get; set; }
    }
}
