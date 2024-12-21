#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DotNet8Authentication.Identity;

[Index("RoleId", Name = "IX_AspNetRoleClaims_RoleId")]
public partial class AspNetRoleClaims
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string RoleId { get; set; }

    public string ClaimType { get; set; }

    public string ClaimValue { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("AspNetRoleClaims")]
    public virtual AspNetRoles Role { get; set; }
}