﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace BaseProject.Service.ApiPermissionsService.ViewModels
{
    public class ApiPermissionsSaveVM
    {
        public Guid? RoleId { get; set; }
        public Guid? UserId { get; set; }
        public bool FullPermission { get; set; }
        public List<string>? Paths { get; set; }
        public List<string>? Controllers { get; set; }
    }
}
