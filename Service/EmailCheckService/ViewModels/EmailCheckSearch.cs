using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseProject.Service.Dto;

namespace BaseProject.Service.EmailCheckService.ViewModels
{
    public class EmailCheckSearch :SearchBase
    {
    
        public int id { get; set; }
        [MaxLength(500)]
        public string? title { get; set; }
        public string? content { get; set; }
        [MaxLength(255)]
        public string? from_email { get; set; }
        [MaxLength(255)]
        public string? to_email { get; set; }
        public DateTime received_time { get; set; }
        [MaxLength(50)]
        public string? category { get; set; }
        public string? suspicious_indicators { get; set; }
        public DateTime created_at { get; set; }
    }
}
