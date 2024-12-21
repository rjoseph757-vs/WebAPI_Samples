#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DotNet8Authentication.Identity;

[PrimaryKey("UserId", "RoleId")]
[Index("RoleId", Name = "IX_AspNetUserRoles_RoleId")]
public partial class AspNetUserRoles
{
    [Key]
    public string UserId { get; set; }

    [Key]
    public string RoleId { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("AspNetUserRoles")]
    public virtual AspNetRoles Role { get; set; }
}