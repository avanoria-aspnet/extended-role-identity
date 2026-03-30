namespace Application.Common.Results;

public sealed record AuthResult
(
    bool Succeeded,
    string? ErrorMessage = null
);
