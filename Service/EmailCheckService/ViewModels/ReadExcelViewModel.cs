using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseProject.Service.EmailCheckService.Dto;

namespace BaseProject.Service.EmailCheckService.ViewModels
{
    public class EmailCheckResponseImportExcel
    {
        public List<EmailCheckImportItemDto> ListEmailCheck { get; set; } = new List<EmailCheckImportItemDto>();
        public int SoLuongThanhCong { get; set; }
        public int SoLuongThatBai { get; set; }
    }

    public class EmailCheckImportItemDto
    {
        public EmailCheckDto Data { get; set; }
        public int RowIndex { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public bool IsValid => Errors == null || Errors.Count == 0;
    }
}
