﻿using BaseProject.Model.Entities;

namespace BaseProject.Service.AppUserService.Dto
{
    public class LoginResponseDto
    {
        public AppUserDto? User { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? Expire { get; set; }

        public bool? IsSSO { get; set; }
    }

    public class LoginResponseSSODto
    {
        public string Data { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }

    public class DataObject
    {
        public UserInfoSSO Data { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }

    public class UserInfoSSO
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string HoTen { get; set; }
        public string DonViID { get; set; }
        public string TenDonVi { get; set; }
        public string TenPhongBan { get; set; }
        public string TenChucVu { get; set; }
        public List<string> DonViKhac { get; set; }
    }

}
