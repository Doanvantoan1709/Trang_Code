using System;
using System.Collections.Generic;

namespace BaseProject.Model.Entities;

public partial class Permission
{
    public int PermissionId { get; set; }

    public int AccountId { get; set; }

    public int FunctionId { get; set; }

    public bool? IsView { get; set; }

    public bool? IsInsert { get; set; }

    public bool? IsUpdate { get; set; }

    public bool? IsDelete { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Function Function { get; set; } = null!;
}
