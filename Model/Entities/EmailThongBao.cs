﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Model.Entities
{
    [Table("EmailThongBao")]
    public class EmailThongBao : AuditableEntity
    {
        [Required]
        [StringLength(255)]
        public string? Ma { get; set; } 
        public string? NoiDung { get; set; }
    }
}
