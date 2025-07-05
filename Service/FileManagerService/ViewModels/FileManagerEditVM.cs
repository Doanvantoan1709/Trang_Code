using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace BaseProject.Service.FileManagerService.ViewModels
{
    public class FileManagerEditVM : FileManagerCreateVM
    {
        public Guid? Id { get; set; }
    }
}
