using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace BaseProject.Service.FileSecurityService.ViewModels
{
    public class FileSecurityEditVM : FileSecurityCreateVM
    {
        public Guid? Id { get; set; }
    }
}
