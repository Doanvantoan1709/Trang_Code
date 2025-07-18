using System;
using System.Collections.Generic;

namespace BaseProject.Model.Entities;

public partial class Account
{
    public int AccountId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Address { get; set; }

    public string? Fullname { get; set; }

    public bool? IsAdmin { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? ExpiredTime { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
