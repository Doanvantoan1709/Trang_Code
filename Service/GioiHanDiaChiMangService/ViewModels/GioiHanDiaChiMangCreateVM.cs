﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace BaseProject.Service.GioiHanDiaChiMangService.ViewModels
{
    public class GioiHanDiaChiMangCreateVM
    {
        [Required]
		public string IPAddress {get; set; }
		[Required]
		public bool Allowed {get; set; }
    }
}
