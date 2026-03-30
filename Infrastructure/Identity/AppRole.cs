using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class AppRole : IdentityRole
{
    public string? Description { get; set; }
}