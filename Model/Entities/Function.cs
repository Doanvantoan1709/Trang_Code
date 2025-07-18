using System;
using System.Collections.Generic;

namespace BaseProject.Model.Entities;

public partial class Function
{
    public int FunctionId { get; set; }

    public string FunctionName { get; set; } = null!;

    public string FunctionCode { get; set; } = null!;

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
