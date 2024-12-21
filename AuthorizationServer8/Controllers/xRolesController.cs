using AuthorizationServer8.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

/// <summary>
/// Controls the actions for roles within the API
/// </summary>
/// <response code="401">If the user did not login correctly or 
/// does not have the correct permissions</response>
[Route("api/[controller]")]
[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[Authorize(Roles = "SuperAdmin")]
public class xRolesController : ControllerBase
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<xRolesController> _logger;

    public xRolesController(
        RoleManager<ApplicationRole> roleManager,
        ILogger<xRolesController> logger
        )
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    /// <summary>
    /// Gets a List of all the Roles
    /// </summary>
    /// <returns>A list of ApplicationRoles</returns>
    /// <response code="200">Returns the list</response>
    /// <response code="404">If the list is empty</response>
    [HttpGet("ListRoles", Name = nameof(ListRolesAsync))]
    [Authorize(AuthenticationSchemes = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IList<ApplicationRole>>> ListRolesAsync()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        if (roles.Count == 0)
        {
            return NotFound();
        }
 
        return Ok(roles);
    }
}