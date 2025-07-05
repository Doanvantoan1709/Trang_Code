﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseProject.Model.Entities
{
    [Table("Notification")]
    public class Notification : AuditableEntity
    {
        [Required]
        public string Message { get; set; }
        public string Link { get; set; }
        public Guid? FromUser { get; set; }
        public Guid? ToUser { get; set; }
        public bool IsRead { get; set; }

        [MaxLength(250)]
        public string Type { get; set; }
        public bool? SendToFrontEndUser { get; set; }
        public Guid? ItemId { get; set; }
        public string ItemName { get; set; }
        public Guid? ItemType { get; set; }
        public bool? IsDisplay { get; set; }
        public Guid? DonViId { get; set; }

        public string? Email { get; set; }
        public string? LoaiThongBao { get; set; }

        public string? ProductId { get; set; }
        public string? ProductName { get; set; }

        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public Guid? NguoiTao { get; set; }
        public string? FileDinhKem { get; set; }
        public bool? IsXuatBan { get; set; }

        public Notification()
        {
        }

        public Notification(Guid fromUser, Guid toUser, string link, string message, string itemName, bool isRead = false)
        {
            this.FromUser = fromUser;
            this.ToUser = toUser;
            this.Link = link;
            this.Message = message;
            this.ItemName = itemName;
            this.Type = itemName;
            this.IsRead = isRead;
            this.TieuDe = message;
            this.NoiDung = message;
        }
    }
}
