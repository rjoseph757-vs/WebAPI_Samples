#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet8Authentication.Identity;

public partial class AspNetRoles
{
    [Key]
    public string Id { get; set; }

    [StringLength(256)]
    public string Name { get; set; }

    [StringLength(256)]
    public string NormalizedName { get; set; }

    public string ConcurrencyStamp { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<AspNetRoleClaims> AspNetRoleClaims { get; set; } = new List<AspNetRoleClaims>();

    [InverseProperty("Role")]
    public virtual ICollection<AspNetUserRoles> AspNetUserRoles { get; set; } = new List<AspNetUserRoles>();
}