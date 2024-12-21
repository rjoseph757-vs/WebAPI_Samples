using WebAPI8CleanArch.Application.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace WebAPI8CleanArch.Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }
}
