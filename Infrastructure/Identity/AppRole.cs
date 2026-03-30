using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class AppRole : IdentityRole
{
    public string? Description { get; set; }

    public static AppRole Create(string roleName, string? description = null)
    {
        return new AppRole
        {
            Id = Guid.NewGuid().ToString(),
            Name = roleName,
            NormalizedName = roleName.ToUpperInvariant(),
            Description = description
        };
    }

    public static AppRole Create(string id, string roleName, string? description = null)
    {
        return new AppRole
        {
            Id = id,
            Name = roleName,
            NormalizedName = roleName.ToUpperInvariant(),
            Description = description
        };
    }
}