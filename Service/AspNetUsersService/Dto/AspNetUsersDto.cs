﻿using BaseProject.Model.Entities;
using BaseProject.Model.Entities;
using System.ComponentModel;

namespace BaseProject.Service.AspNetUsersService.Dto
{
    public class AspNetUsersDto : AppUser
    {
        public string TenDonVi_txt { get; set; }
        public string GioiTinh_txt 
        {
            get
            {
                if(Gender == 1)
                {
                    return "Nam";
                }
                else {
                    return "Nữ";
                }
            }
        }
        public string VaiTro_response { get; set; }
        public List<string> VaiTro_txt_response { get; set; }
        public List<string> ListRole { get; set; }

    }

    public class UserDto
    {
        public Guid Id { get; set; }

        [DisplayName("Tài khoản")]
        public string? UserName { get; set; }

        [DisplayName("Họ tên")]
        public string? Name { get; set; }

        [DisplayName("Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [DisplayName("Email")]
        public string? Email { get; set; }

        [DisplayName("Địa chỉ")]
        public string? DiaChi { get; set; }

        [DisplayName("Ngày sinh")]
        public DateTime? NgaySinh { get; set; }
        public bool LockoutEnabled { get; set; }
        public Guid? DonViId { get; set; }
        public string? Gender { get; set; }
        public string? Picture { get; set; }
        public List<string>? vaiTro { get; set; }
        public List<string>? ListPhongBan { get; set; }

        [DisplayName("Tên đơn vị")]
        public string? TenDonVi_txt { get; set; }

        [DisplayName("Giới tính")]
        public string? GioiTinh_txt { get; set; }

        [DisplayName("Vai trò")]
        public string? VaiTro_response { get; set; }
        public List<string>? VaiTro_txt_response { get; set; }

        public string? GroupRole_txt { get; set; } 
        public List<string>? GroupRole_response { get; set; }

        public string? DepartmentId { get; set; }
        public string? Department_txt { get; set; }

        public List<Guid>? NhomNguoi { get; set; }
        public List<string>? NhomNguoi_txt { get; set; }

    }
}
