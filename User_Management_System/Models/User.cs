using System;
using System.Collections.Generic;

namespace User_Management_System.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? FullName { get; set; }

    public int RoleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Information> Information { get; set; } = new List<Information>();

    public virtual Role Role { get; set; } = null!;
}
