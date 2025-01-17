﻿#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DotNet8Authentication.Identity;

[Index("UserId", Name = "IX_AspNetUserClaims_UserId")]
public partial class AspNetUserClaims
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    public string ClaimType { get; set; }

    public string ClaimValue { get; set; }
}