using BaseProject.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class BaseProjectContext : DbContext
    {
        public BaseProjectContext(DbContextOptions<BaseProjectContext> options)
            : base(options)
        {
        }

        public DbSet<Account> account { get; set; }
        public DbSet<Permission> permission { get; set; }
        public DbSet<Function> function { get; set; }

        public DbSet<Role> Role { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<ApiPermissions> ApiPermissions { get; set; }
        public DbSet<Module> Module { get; set; }
        public DbSet<Operation> Operation { get; set; }
        public DbSet<RoleOperation> RoleOperation { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<TaiLieuDinhKem> TaiLieuDinhKem { get; set; }
        public DbSet<Audit> Audit { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<FileManager> FileManager { get; set; }
        public DbSet<FileSecurity> FileSecurity { get; set; }
        public DbSet<EmailThongBao> EmailThongBao { get; set; }
        public DbSet<QLThongBao> QLThongBao { get; set; }
        public DbSet<TestScaffold> TestScaffold { get; set; }

        // tạm ko dùng GroupRole
        //public DbSet<GroupRole> GroupRole { get; set; }

        // nhóm người
        public DbSet<GroupUser> GroupUser { get; set; }
        public DbSet<User_GroupUser> User_GroupUser { get; set; }

        // nhóm người - nhóm quyền
        public DbSet<GroupUserRole> GroupUserRole { get; set; }
        public DbSet<SystemLogs> SystemLogs { get; set; }
        public DbSet<GioiHanDiaChiMang> GioiHanDiaChiMang { get; set; }

        //Đơn thư
    }
}
