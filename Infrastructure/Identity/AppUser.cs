using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ImageUrl { get; set; }


    public static AppUser Create(string email, bool emailConfirmed = false, string? firstName = null, string? lastName = null, string? imageUrl = null)
    {
        return new AppUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            ImageUrl = imageUrl,
            EmailConfirmed = emailConfirmed
        };
    }
}
