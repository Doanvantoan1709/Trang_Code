﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Service.RoleService.ViewModels
{
    public class RoleVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }

        public bool IsChecked { get; set; } = false;
    }
}
