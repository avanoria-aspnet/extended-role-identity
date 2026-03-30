namespace Application.Common.Results;

public sealed record AccountResult
(
    bool Succeeded,
    string? ErrorMessage = null,
    AppUser?
)
{
    public static AccountResult Ok() => new(true);
    public static AccountResult Failed(string errorMessage) => new(false, errorMessage);
    public static AccountResult NotFound(string errorMessage = "User not found") => new(false, errorMessage);
};
