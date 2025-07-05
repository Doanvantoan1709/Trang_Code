using BaseProject.Model.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BaseProject.Service.TaiLieuDinhKemService.Dto
{
    public class UploadFileDto
    {
        [Required]
        public IFormFileCollection? Files { get; set; }
        public string? FileType { get; set; } = "";
        public Guid? ItemId { get; set; }
        public bool? IsTemp { get; set; } = false;
        public string? MoTa { get; set; } = "";
        public Guid? IdBieuMau { get; set; }
    }
}
