#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DotNet8Authentication.Identity;

[PrimaryKey("UserId", "LoginProvider", "Name")]
public partial class AspNetUserTokens
{
    [Key]
    public string UserId { get; set; }

    [Key]
    public string LoginProvider { get; set; }

    [Key]
    public string Name { get; set; }

    public string Value { get; set; }
}