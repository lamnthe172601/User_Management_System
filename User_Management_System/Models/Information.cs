using System;
using System.Collections.Generic;

namespace User_Management_System.Models;

public partial class Information
{
    public int InfoId { get; set; }

    public int UserId { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public bool Gender { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual User User { get; set; } = null!;
}
